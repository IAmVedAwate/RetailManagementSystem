using Microsoft.AspNetCore.Mvc;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.DTO;
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

        public ApiResponse GetBillsSV(int userId)
        {
            var billOwnerId = userId;
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

        public ApiResponse GetBillSV(string index, int userId)
        {
            var billOwnerId = userId;
            if (billOwnerId != 0)
            {
                Bill bill = _unitOfWork.Bill.Get(u => u.StoreId == billOwnerId && u.indexForBill == index, includeProperties: ["StoreUser"]);
                if (bill != null)
                {
                    _response.Result = _unitOfWork.Order.GetAll(u => u.BillId == bill.Id, includeProperties: ["Bill"]);
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

        public ApiResponse CreateBillSV(BillDTO createBillDTO, int userId)
        {
            try
            {
                
                // Create Warehouse Object Here
                Bill newBill = new Bill();
                var billOwnerId = userId;
                if (billOwnerId != 0)
                {
                    newBill.TotalAmount = (float)createBillDTO.orders.Select(order => order.TotalAmount).Sum();
                    newBill.DateGenerated = DateTime.Now;
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
                _response.Result = orderEntities;
                _response.StatusCode = HttpStatusCode.Created;
               
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return (_response);
            }
            _unitOfWork.Save();
            return (_response);
        }

        public ApiResponse UpdateBillSV(int id, OrderDTO updateBillDTO, int userId)
        {
            try
            {
                var billOwnerId = userId;
                if (billOwnerId != 0)
                {
                    Order orderFromDb = _unitOfWork.Order.Get(u => u.Id == id);
                    IEnumerable<Order> orderEntries = _unitOfWork.Order.GetAll(u => u.BillId == orderFromDb.BillId);
                    Bill billFromDb = _unitOfWork.Bill.Get(u => u.Id == orderFromDb.BillId);
                    orderFromDb.StockId = updateBillDTO.StockId;
                    orderFromDb.Quantity = updateBillDTO.Quantity;
                    orderFromDb.TotalAmount = updateBillDTO.TotalAmount;

                    billFromDb.TotalAmount = (float)orderEntries.Select(order => order.TotalAmount).Sum();

                    _unitOfWork.Order.Update(orderFromDb);
                    _unitOfWork.Bill.Update(billFromDb);

                    _response.Result = updateBillDTO;
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
            _unitOfWork.Save();
            return (_response);
        }
        public ApiResponse DeleteOrderSV(int id, int userId)
        {
            try
            {
                Order orderFromDb = _unitOfWork.Order.Get(u => u.Id == id);
                IEnumerable<Order> orderEntries = _unitOfWork.Order.GetAll(u => u.BillId == orderFromDb.BillId);
                Bill billFromDb = _unitOfWork.Bill.Get(u => u.Id == orderFromDb.BillId);
                var billOwnerId = userId;
                if (billFromDb.StoreId == billOwnerId)
                {
                    billFromDb.TotalAmount = (float)orderEntries.Select(order => order.TotalAmount).Sum() - orderFromDb.TotalAmount;

                    _unitOfWork.Order.Remove(orderFromDb);
                    _unitOfWork.Bill.Update(billFromDb);

                    _unitOfWork.Save();
                    _response.Result = "Deleted Successfully !! ";
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

        public ApiResponse DeleteBillSV(int id, int userId)
        {
            Bill billFromDb = _unitOfWork.Bill.Get(u => u.Id == id);
            var billOwnerId = userId;
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
