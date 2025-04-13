using Microsoft.AspNetCore.Mvc;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.Delivery;
using RetailManagementSystem.Models.Models.DTO;
using RetailManagementSystem.Models.Models.ResultModels;
using RetailManagementSystem.Models.Models.Retailer;
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
    public class DeliveryServices : IDeliveryServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApiResponse _response;
        public DeliveryServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new ApiResponse();
        }
        public ApiResponse GetDeliveriesSV()
        {
            IEnumerable<Delivery> AllDeliveries = _unitOfWork.Delivery.GetAll(includeProperties: ["Bill.StoreUser"]);
            IEnumerable<Order> AllOrders = _unitOfWork.Order.GetAll(includeProperties: ["Stock.Product.SubCategory", "Stock.Warehouse", "Bill"]);
            List<DeliveryDetailedResult> CombinedResult = AllDeliveries.Select(delivery =>
            {
                List<OrderFromBill> OrdersFromBill = AllOrders.Where(order => order.BillId == delivery.BillId).Select(order =>
                {
                    ProductData product = new ProductData()
                    {
                        ProductName = order.Stock.Product.ProductName,
                        ProductDescription = order.Stock.Product.ProductDescription,
                        RetailPrice = order.Stock.Product.RetailPrice,
                        MRP = order.Stock.Product.MRP,
                        Category = order.Stock.Product.SubCategory.SubCategoryName
                    };

                    StockData stockData = new StockData()
                    {
                        Index = order.Stock.IndexForDeletion,
                        Product = product,
                        Quantity = order.Stock.Quantity,
                        MarginPercentage = order.Stock.MarginPercentage,
                        IsReturnable = order.Stock.IsReturnable,
                    };
                    RetailerUser retailer = _unitOfWork.RetailerUser.Get(u => u.WarehouseId == order.Stock.WarehouseId);
                    RetailerData Retailer = new RetailerData()
                    {
                        RetailName = retailer.RetailName,
                        OwnerName = retailer.OwnerName,
                        Phone = retailer.Phone,
                        Address = retailer.Address,
                    };
                    OrderFromBill orderFromBill = new OrderFromBill()
                    {
                        Stock = stockData,
                        Quantity = order.Quantity,
                        TotalAmount = order.TotalAmount,
                        Retailer = Retailer
                    };
                    return orderFromBill;
                }).ToList();

                StoreData store = new StoreData()
                {
                    IndexForBill = delivery.Bill.indexForBill,
                    StoreName = delivery.Bill.StoreUser.StoreName,
                    PhotoOfStore = delivery.Bill.StoreUser.PhotoOfStore,
                    ProfilePhoto = delivery.Bill.StoreUser.ProfilePhoto,
                    Phone = delivery.Bill.StoreUser.Phone,
                    Address = delivery.Bill.StoreUser.Address
                };


                return new DeliveryDetailedResult
                {
                    DeliveryId = delivery.Id,
                    Status = delivery.Status,
                    Instructions = delivery.Instructions,
                    GoogleMapLocation = delivery.GoogleMapLocation,
                    storeData = store,
                    OrdersFromBill = OrdersFromBill
                };
            }).ToList();
            _response.Result = CombinedResult;
            _response.StatusCode = HttpStatusCode.OK;
            return (_response);
        }

        public ApiResponse GetReturnSV(string index)
        {
            Return returnObj = _unitOfWork.Return.Get(u => u.indexForReturn.ToString() == index);
            _response.Result = returnObj;
            _response.StatusCode = HttpStatusCode.OK;
            return (_response);
        }
        public ApiResponse CreateDeliverySV(DeliveryDTO deliveryDTO, string email)
        {
            try
            {
                // Find the Bill using the deliveryDTO's BillIndex.
                Bill billFromDb = _unitOfWork.Bill.Get(u => u.indexForBill == deliveryDTO.BillIndex);
                if (billFromDb == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "Bill not found." };
                    return _response;
                }

                // Create a new Delivery record.
                Delivery delivery = new Delivery()
                {
                    BillId = billFromDb.Id,
                    Status = deliveryDTO.Status,
                    //DeliveryDate = deliveryDTO.DeliveryDate,
                    Instructions = deliveryDTO.Instructions,
                    Phone2 = deliveryDTO.Phone2,
                    GoogleMapLocation = deliveryDTO.GoogleMapLocation,
                };

                _unitOfWork.Delivery.Add(delivery);

                // Retrieve all orders associated with this Bill.
                IEnumerable<Order> orders = _unitOfWork.Order.GetAll(o => o.BillId == billFromDb.Id);

                // For each order, locate its associated stock, then reduce its quantity.
                foreach (var order in orders)
                {
                    // Get the corresponding Stock using the Order's StockId.
                    Stock stock = _unitOfWork.Stock.Get(s => s.Id == order.StockId);
                    if (stock != null)
                    {
                        // Optionally add validation if stock.Quantity is insufficient.
                        if (stock.Quantity >= order.Quantity)
                        {
                            stock.Quantity -= order.Quantity;
                        }
                        else
                        {
                            // Optionally: handle insufficient stock situation.
                            order.Quantity = stock.Quantity;
                            stock.Quantity = 0; // or throw an exception or add a warning message.
                            _unitOfWork.Order.Update(order);
                        }
                        _unitOfWork.Stock.Update(stock);
                        
                    }
                }

                // Set the response properties.
                _response.Result = delivery;
                _response.StatusCode = HttpStatusCode.Created;

                // Save changes to the database for both the delivery and stock updates.
                _unitOfWork.Save();
                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return _response;
            }
        }
        public ApiResponse AssignedDeliveryByIdSV(int id, string email)
        {
            var deliveryUserId = 0;
            if (email != null)
            {
                var DeliveryOwner = _unitOfWork.DeliveryUser.Get(u => u.Email == email);
                deliveryUserId = DeliveryOwner.Id;
            }
            OrderAssign oneDelivery = _unitOfWork.OrderAssign.Get(u=>u.DeliveryId == id && u.DeliveryUserId == deliveryUserId,includeProperties: ["Delivery.Bill.StoreUser"]);
            IEnumerable<Order> AllOrders = _unitOfWork.Order.GetAll(includeProperties: ["Stock.Product.SubCategory", "Stock.Warehouse", "Bill"]);
            List<OrderFromBill> OrdersFromBill = AllOrders.Where(order => order.BillId == oneDelivery.Delivery.BillId).Select(order =>
            {
                ProductData product = new ProductData()
                {
                    ProductName = order.Stock.Product.ProductName,
                    ProductDescription = order.Stock.Product.ProductDescription,
                    RetailPrice = order.Stock.Product.RetailPrice,
                    MRP = order.Stock.Product.MRP,
                    Category = order.Stock.Product.SubCategory.SubCategoryName
                };

                StockData stockData = new StockData()
                {
                    Index = order.Stock.IndexForDeletion,
                    Product = product,
                    Quantity = order.Stock.Quantity,
                    MarginPercentage = order.Stock.MarginPercentage,
                    IsReturnable = order.Stock.IsReturnable,
                };
                RetailerUser retailer = _unitOfWork.RetailerUser.Get(u => u.WarehouseId == order.Stock.WarehouseId);
                RetailerData Retailer = new RetailerData()
                {
                    RetailName = retailer.RetailName,
                    OwnerName = retailer.OwnerName,
                    Phone = retailer.Phone,
                    Address = retailer.Address,
                };
                OrderFromBill orderFromBill = new OrderFromBill()
                {
                    Stock = stockData,
                    Quantity = order.Quantity,
                    TotalAmount = order.TotalAmount,
                    Retailer = Retailer
                };
                return orderFromBill;
            }).ToList();

            StoreData store = new StoreData()
            {
                IndexForBill = oneDelivery.Delivery.Bill.indexForBill,
                StoreName = oneDelivery.Delivery.Bill.StoreUser.StoreName,
                PhotoOfStore = oneDelivery.Delivery.Bill.StoreUser.PhotoOfStore,
                ProfilePhoto = oneDelivery.Delivery.Bill.StoreUser.ProfilePhoto,
                Phone = oneDelivery.Delivery.Bill.StoreUser.Phone,
                Address = oneDelivery.Delivery.Bill.StoreUser.Address
            };
            DeliveryDetailedResult CombinedResult = new();

            CombinedResult.DeliveryId = oneDelivery.Delivery.Id;
            CombinedResult.Status = oneDelivery.Delivery.Status;
            CombinedResult.Instructions = oneDelivery.Delivery.Instructions;
            CombinedResult.GoogleMapLocation = oneDelivery.Delivery.GoogleMapLocation;
            CombinedResult.storeData = store;
            CombinedResult.OrdersFromBill = OrdersFromBill;
                
            
            _response.Result = CombinedResult;
            _response.StatusCode = HttpStatusCode.OK;
            return (_response);
        }

        public ApiResponse AssignDeliveryToUserSV(int id, string email)
        {
            var deliveryUserId = 0;
            if (email != null)
            {
                var DeliveryOwner = _unitOfWork.DeliveryUser.Get(u => u.Email == email);
                deliveryUserId = DeliveryOwner.Id;
            }
            try
            {
                if (id != _unitOfWork.OrderAssign.Get(u=>u.DeliveryId == id)?.DeliveryId)
                {
                    OrderAssign orderAssign = new OrderAssign();
                    orderAssign.DeliveryId = id;
                    orderAssign.DeliveryUserId = deliveryUserId;
                    _unitOfWork.OrderAssign.Add(orderAssign);

                    Delivery delivery = _unitOfWork.Delivery.Get(u => u.Id == id);
                    delivery.Status = "Assigned";
                    _unitOfWork.Delivery.Update(delivery);
                    _unitOfWork.Save();
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.Forbidden;
                    _response.ErrorMessages = new List<string>() { "This Delivery Already Assigned to Someone!" };
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return (_response);
            }
            return _response;
        }

        public ApiResponse SubmitReturnsSV(ReturnDTO submitReturnsDTO, string email)
        {
            try
            {

                var deliveryUserId = 0;
                if (email != null)
                {
                    var DeliveryOwner = _unitOfWork.DeliveryUser.Get(u => u.Email == email);
                    deliveryUserId = DeliveryOwner.Id;
                }
                Order orderFromDb = _unitOfWork.Order.Get(u => u.Id == submitReturnsDTO.OrderId, includeProperties: ["Stock"]);
                var billIdFromDb = _unitOfWork.Order.Get(u => u.Id == submitReturnsDTO.OrderId).BillId;
                var deliveryUserIdFromDb = _unitOfWork.OrderAssign.Get(u => u.Delivery.BillId == billIdFromDb).DeliveryUserId;
                if (deliveryUserId != 0 && deliveryUserId == deliveryUserIdFromDb)
                {
                    if (orderFromDb.Stock.IsReturnable)
                    {
                        Return returns = new Return()
                        {
                            OrderId = submitReturnsDTO.OrderId,
                            indexForReturn = Guid.NewGuid(),
                            DeliveryUserId = deliveryUserId,
                            ReturnReason = submitReturnsDTO.ReturnReason,
                            ReturnStatus = submitReturnsDTO.ReturnStatus,
                            ApprovalDate = DateTime.Now,
                            RefundAmmount = orderFromDb.TotalAmount,
                            Comments = submitReturnsDTO.Comments,
                            ReturnMethod = submitReturnsDTO.ReturnMethod,
                            PhotoEvidence = submitReturnsDTO.PhotoEvidence

                        };
                        _unitOfWork.Return.Add(returns);
                        _response.Result = returns;
                        _response.StatusCode = HttpStatusCode.Created;
                    }
                    else
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.Forbidden;
                        _response.ErrorMessages = ["Not Returnable Product, So Can't Return it, Sorry!!! "];
                        return (_response);
                    }

                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.Unauthorized;
                    return (_response);
                }

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return (_response);
            }
            _unitOfWork.Save();
            return (_response);
        }
    }
}
