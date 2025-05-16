using Microsoft.AspNetCore.Mvc;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.DTO;
using RetailManagementSystem.Models.Models.ResultModels;
using RetailManagementSystem.Models.Models.Store;
using RetailManagementSystem.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Services
{
    public class BillServices : IBillServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApiResponse _response;
        public BillServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new ApiResponse();
        }

        public ApiResponse GetBillsSV(string email)
        {
            var billOwnerId = 0;
            if (email != null)
            {
                var BillOwner = _unitOfWork.StoreUser.Get(u => u.Email == email);
                if(BillOwner!=null) billOwnerId = BillOwner.Id;
            }
            if (billOwnerId != 0)
            {
                _response.Result = _unitOfWork.Bill.GetAll(u => u.StoreId == billOwnerId, includeProperties: ["StoreUser"]);
                _response.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                _response.StatusCode = HttpStatusCode.Unauthorized;
                _response.IsSuccess = false;
            }
            return (_response);
        }

        public ApiResponse GetReturnableSV()
        {
            // Fetch all bills with their related StoreUser
            var bills = _unitOfWork.Bill.GetAll(includeProperties: ["StoreUser"]);

            // Project to the required fields
            var result = bills.Select(b => new
            {
                billId = b.indexForBill,
                billName = b.billName,
                email = b.StoreUser?.Email,
                storeName = b.StoreUser?.StoreName
            }).ToList();

            _response.Result = result;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            return _response;
        }

        public ApiResponse GetOrderForReturnSV(int orderId)
        {
            var order = _unitOfWork.Order.Get(u => u.Id == orderId);
            if (order != null)
            {
                _response.Result = order;
                _response.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                _response.Result = null;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { "Order not found." };
            }
            return _response;
        }

        public ApiResponse GetBillSV(string index, string email)
        {
            var billOwnerId = 0;
            if (email != null)
            {
                var BillOwner = _unitOfWork.StoreUser.Get(u => u.Email == email);
                billOwnerId = BillOwner.Id;
            }
            if (billOwnerId != 0)
            {
                Bill bill = _unitOfWork.Bill.Get(u => u.StoreId == billOwnerId && u.indexForBill == index, includeProperties: ["StoreUser"]);
                if (bill != null)
                {
                    _response.Result = _unitOfWork.Order.GetAll(u => u.BillId == bill.Id, includeProperties: ["Bill","Stock.Product.SubCategory", "Stock.Warehouse"]);
                    _response.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.Unauthorized;
                    _response.IsSuccess = false;
                }
            }
            else
            {
                _response.StatusCode = HttpStatusCode.Unauthorized;
                _response.IsSuccess = false;
            }
            return (_response);
        }

        public ApiResponse CreateBillSV(BillDTO createBillDTO, string email)
        {
            Bill newBill = new Bill();
            try
            {
                
                // Create Warehouse Object Here
                
                var billOwnerId = 0;
                if (email != null)
                {
                    var BillOwner = _unitOfWork.StoreUser.Get(u => u.Email == email);
                    billOwnerId = BillOwner.Id;
                }
                if (billOwnerId != 0)
                {
                    newBill.TotalAmount = (float)createBillDTO.orders.Select(order => order.TotalAmount).Sum();
                    newBill.DateGenerated = DateTime.Now;
                    newBill.billName = createBillDTO.billName;
                    newBill.StoreId = billOwnerId;
                    _unitOfWork.Bill.Add(newBill);
                    _unitOfWork.Save();
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.Unauthorized;
                    _response.IsSuccess = false;
                }

                var orderEntities = createBillDTO.orders.Select(order => new Order
                {
                    StockId = order.StockId,
                    Quantity = order.Quantity,
                    TotalAmount = order.TotalAmount,
                    BillId = newBill.Id,
                }).ToList();

                _unitOfWork.Order.AddRange(orderEntities);
                _unitOfWork.Save();
                _response.Result = createBillDTO.orders.Select(order => new OrdersWithIndex
                {
                    StockId = order.StockId,
                    Quantity = order.Quantity,
                    TotalAmount = order.TotalAmount,
                    BillId = newBill.Id,
                    BillIndex = newBill.indexForBill,
                }).ToList(); ;
                _response.StatusCode = HttpStatusCode.Created;
               
            }
            catch (Exception ex)
            {
                _unitOfWork.Bill.Remove(newBill);
                _unitOfWork.Save();
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return (_response);
            }
            _unitOfWork.Save();
            return (_response);
        }

        public ApiResponse UpdateBillSV(int id, OrderDTO updateBillDTO, string email)
        {
            try
            {
                var billOwnerId = 0;
                if (email != null)
                {
                    var BillOwner = _unitOfWork.StoreUser.Get(u => u.Email == email);
                    billOwnerId = BillOwner.Id;
                }
                if (billOwnerId != 0)
                {
                    Order orderFromDb = _unitOfWork.Order.Get(u => u.Id == id);
                    if (orderFromDb == null || id == 0)
                    {
                        orderFromDb = new Order();
                        orderFromDb.BillId = updateBillDTO.BillId;
                    } 
                    IEnumerable<Order> orderEntries = _unitOfWork.Order.GetAll(u => u.BillId == updateBillDTO.BillId);
                    Bill billFromDb = _unitOfWork.Bill.Get(u => u.Id == updateBillDTO.BillId);
                    orderFromDb.StockId = updateBillDTO.StockId;
                    orderFromDb.Quantity = updateBillDTO.Quantity;
                    orderFromDb.TotalAmount = updateBillDTO.TotalAmount;

                    billFromDb.TotalAmount = (float)orderEntries.Select(order => order.TotalAmount).Sum();
                    if (orderFromDb == null || id == 0) 
                        _unitOfWork.Order.Add(orderFromDb);
                    else 
                        _unitOfWork.Order.Update(orderFromDb);
                    _unitOfWork.Save();

                    _unitOfWork.Bill.Update(billFromDb);
                    _unitOfWork.Save();


                    _response.Result = orderFromDb;
                    _response.StatusCode = HttpStatusCode.Created;
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.Unauthorized;
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return (_response);
            }
            return (_response);
        }
        public ApiResponse DeleteOrderSV(int id, string email)
        {
            try
            {
                Order orderFromDb = _unitOfWork.Order.Get(u => u.Id == id);
                IEnumerable<Order> orderEntries = _unitOfWork.Order.GetAll(u => u.BillId == orderFromDb.BillId);
                Bill billFromDb = _unitOfWork.Bill.Get(u => u.Id == orderFromDb.BillId);
                var billOwnerId = 0;
                if (email != null)
                {
                    var BillOwner = _unitOfWork.StoreUser.Get(u => u.Email == email);
                    billOwnerId = BillOwner.Id;
                }
                if (billFromDb.StoreId == billOwnerId)
                {
                    int orderCount = orderEntries.Count();
                    _unitOfWork.Order.Remove(orderFromDb);

                    if (orderCount == 1)
                    {
                        // This was the last order, so remove the bill as well
                        _unitOfWork.Bill.Remove(billFromDb);
                        _unitOfWork.Save();
                        _response.Result = "Order and Bill deleted successfully!";
                    }
                    else
                    {
                        // There are still other orders, just update the bill
                        billFromDb.TotalAmount = (float)orderEntries.Select(order => order.TotalAmount).Sum() - orderFromDb.TotalAmount;
                        _unitOfWork.Bill.Update(billFromDb);
                        _unitOfWork.Save();
                        _response.Result = "Order deleted successfully!";
                    }
                    _response.StatusCode = HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return (_response);
            }
            return (_response);
        }

        public ApiResponse DeleteBillSV(int id, string email)
        {
            Bill billFromDb = _unitOfWork.Bill.Get(u => u.Id == id);
            var billOwnerId = 0;
            if (email != null)
            {
                var BillOwner = _unitOfWork.StoreUser.Get(u => u.Email == email);
                billOwnerId = BillOwner.Id;
            }
            if (billFromDb.StoreId == billOwnerId)
            {
                IEnumerable<Order> orders = _unitOfWork.Order.GetAll(u => u.BillId == billFromDb.Id);
                _unitOfWork.Order.RemoveRange(orders);
                _unitOfWork.Bill.Remove(billFromDb);
                _unitOfWork.Save();
                _response.Result = "Deleted Successfully !! ";
                _response.StatusCode = HttpStatusCode.OK;
            }

            return (_response);
        }
    }
}
