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
        // 1. Private helper to map a Delivery + Orders → DeliveryDetailedResult
        private DeliveryDetailedResult BuildDeliveryDetail(Delivery delivery, IEnumerable<Order> allOrders)
        {
            // build the orders list for this delivery’s bill
            var ordersFromBill = allOrders
                .Where(o => o.BillId == delivery.BillId)
                .Select(order =>
                {
                    var p = order.Stock.Product;
                    var product = new ProductData
                    {
                        ProductName = p.ProductName,
                        ProductDescription = p.ProductDescription,
                        RetailPrice = p.RetailPrice,
                        MRP = p.MRP,
                        Category = p.SubCategory.SubCategoryName
                    };

                    var stock = order.Stock;
                    var stockData = new StockData
                    {
                        Index = stock.IndexForDeletion,
                        Product = product,
                        Quantity = stock.Quantity,
                        MarginPercentage = stock.MarginPercentage,
                        IsReturnable = stock.IsReturnable,
                    };

                    var retailerUser = _unitOfWork.RetailerUser
                        .Get(r => r.WarehouseId == stock.WarehouseId);
                    var retailer = new RetailerData
                    {
                        RetailName = retailerUser.RetailName,
                        OwnerName = retailerUser.OwnerName,
                        Phone = retailerUser.Phone,
                        Address = retailerUser.Address,
                    };

                    return new OrderFromBill
                    {
                        Stock = stockData,
                        Quantity = order.Quantity,
                        TotalAmount = order.TotalAmount,
                        Retailer = retailer
                    };
                })
                .ToList();

            // build the store info
            var storeUser = delivery.Bill.StoreUser;
            var storeData = new StoreData
            {
                IndexForBill = delivery.Bill.indexForBill,
                StoreName = storeUser.StoreName,
                PhotoOfStore = storeUser.PhotoOfStore,
                ProfilePhoto = storeUser.ProfilePhoto,
                Phone = storeUser.Phone,
                Address = storeUser.Address
            };

            // assemble the result
            return new DeliveryDetailedResult
            {
                DeliveryId = delivery.Id,
                Status = delivery.Status,
                Instructions = delivery.Instructions,
                GoogleMapLocation = delivery.GoogleMapLocation,
                storeData = storeData,
                OrdersFromBill = ordersFromBill
            };
        }
        public ApiResponse GetDeliveriesSV()
        {
            // grab all deliveries and orders once
            IEnumerable<Delivery> allDeliveries = _unitOfWork.Delivery
                .GetAll(includeProperties: [ "Bill.StoreUser" ]);
            IEnumerable<Order> allOrders = _unitOfWork.Order.GetAll(includeProperties: ["Stock.Product.SubCategory","Stock.Warehouse","Bill"]);

            List<DeliveryDetailedResult> combined = allDeliveries
                .Where(d => d.Status == "Pending")
                .Select(d => BuildDeliveryDetail(d, allOrders))
                .ToList();

            _response.Result = combined;
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }
        public ApiResponse GetAcceptedDeliveriesSV(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var user = _unitOfWork.DeliveryUser.Get(u => u.Email == email);
            }
            // grab all deliveries and orders once
            IEnumerable<OrderAssign> allDeliveries = _unitOfWork.OrderAssign
                .GetAll(includeProperties: ["Delivery.Bill.StoreUser"]);
            IEnumerable<Order> allOrders = _unitOfWork.Order.GetAll(includeProperties: ["Stock.Product.SubCategory", "Stock.Warehouse", "Bill"]);

            List<DeliveryDetailedResult> combined = allDeliveries
                .Select(d => BuildDeliveryDetail(d.Delivery, allOrders))
                .ToList();

            _response.Result = combined;
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }

        public ApiResponse GetReturnSV(string email)
        {
            int userId = 0;
            if (!string.IsNullOrEmpty(email))
            {
                userId = _unitOfWork.RetailerUser.Get(u => u.Email == email).WarehouseId;
            }
            IEnumerable<Return> returnObj = _unitOfWork.Return.GetAll(u=> u.Order.Stock.WarehouseId == userId,includeProperties: ["Order.Stock.Warehouse"]);
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
                    OrderPlacedDate = DateTime.Now
                };

                _unitOfWork.Delivery.Add(delivery);

                // Retrieve all orders associated with this Bill.
                IEnumerable<Order> orders = _unitOfWork.Order.GetAll(o => o.BillId == billFromDb.Id);

                // For each order, locate its associated stock, then reduce its quantity.
                foreach (var order in orders)
                {
                    // Get the corresponding Stock using the Order's StockId.
                    Stock stock = _unitOfWork.Stock.Get(s => s.Id == order.StockId, includeProperties: ["Product.SubCategory.Category"]);
                    if (stock != null)
                    {
                        stock.Quantity -= order.Quantity;
                        // If order quantity is greater than stock, update order quantity to match stock
                        if (order.Quantity > stock.Quantity)
                        {
                            order.Quantity = stock.Quantity;
                            _unitOfWork.Order.Update(order);
                            _unitOfWork.Save();
                        }

                        if (stock.Quantity <= 10 && stock.Quantity > 0)
                        {
                            _unitOfWork.RetailMessages.Add(new RetailMessages()
                            {
                                Message = $"Stock is Lower Than 10 for {stock.Product.ProductName} ( Category: {stock.Product.SubCategory.Category.CategoryName}, Subcategory: {stock.Product.SubCategory.SubCategoryName} ) ",
                                WarehouseId = stock.WarehouseId,
                                Type = "W",
                            });
                            _unitOfWork.Save();
                        }

                        if (stock.Quantity <= 0)
                        {
                            // Delete the order and stock if quantity reaches zero
                            _unitOfWork.Order.Remove(order);
                            _unitOfWork.Stock.Remove(stock);
                            // Check if this was the last order for the bill
                            int remainingOrders = _unitOfWork.Order.GetAll(o => o.BillId == order.BillId && o.Id != order.Id).Count();
                            if (remainingOrders == 0)
                            {
                                var bill = _unitOfWork.Bill.Get(b => b.Id == order.BillId);
                                if (bill != null)
                                {
                                    _unitOfWork.Bill.Remove(bill);
                                }
                            }
                            _unitOfWork.RetailMessages.Add(new RetailMessages()
                            {
                                Message = $"Stock is Empty for {stock.Product.ProductName} ( Category: {stock.Product.SubCategory.Category.CategoryName}, Subcategory: {stock.Product.SubCategory.SubCategoryName} ) ",
                                WarehouseId = stock.WarehouseId,
                                Type = "R",
                            });
                            _unitOfWork.Save();
                            continue; // Skip update since it's deleted
                        }
                        else
                        {
                            _unitOfWork.Stock.Update(stock);
                            _unitOfWork.Save();
                        }
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
        public ApiResponse CompleteDeliverySV(int id, string email)
        {
            var deliveryUserId = 0;
            if (email != null)
            {
                var DeliveryOwner = _unitOfWork.DeliveryUser.Get(u => u.Email == email);
                deliveryUserId = DeliveryOwner.Id;
            }
            try
            {
                Delivery delivery = _unitOfWork.Delivery.Get(u => u.Id == id);
                if (delivery != null)
                {
                    delivery.Status = "Completed";
                    delivery.DeliveryDate = DateTime.Now;
                    _unitOfWork.Delivery.Update(delivery);
                    _unitOfWork.Save();
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "Delivery not found." };
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return (_response);
        }
        public ApiResponse AssignedDeliveryByIdSV(int id, string email)
        {
            int userId = 0;
            if (!string.IsNullOrEmpty(email))
            {
                userId = _unitOfWork.DeliveryUser.Get(u => u.Email == email).Id;
            }

            OrderAssign delivery = _unitOfWork.OrderAssign.Get(d => d.DeliveryId == id && d.DeliveryUserId == userId, includeProperties: [ "Delivery.Bill.StoreUser" ]);
            IEnumerable<Order> allOrders = _unitOfWork.Order.GetAll(includeProperties:["Stock.Product.SubCategory","Stock.Warehouse","Bill"]);

            DeliveryDetailedResult result = BuildDeliveryDetail(delivery.Delivery, allOrders);

            _response.Result = result;
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
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
                            ReturnStatus = "Pending",
                            ApprovalDate = DateTime.Now,
                            RefundAmmount = orderFromDb.TotalAmount,
                            Comments = submitReturnsDTO.Comments,
                            ReturnMethod = submitReturnsDTO.ReturnMethod,
                            PhotoEvidence = "submitReturnsDTO.PhotoEvidence"

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

        public ApiResponse RefundAmmountProvideSV(string email)
        {
            var deliveryUserId = 0;
            if (email != null)
            {
                var DeliveryOwner = _unitOfWork.DeliveryUser.Get(u => u.Email == email);
                deliveryUserId = DeliveryOwner.Id;
            }
            OrderAssign dbDelivery = _unitOfWork.OrderAssign.Get(u => u.DeliveryUserId == deliveryUserId);
            _response.Result = dbDelivery;
            return _response;
        }
    }
}
