using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.Store;
using RetailManagementSystem.Models.Models;
using System.Net;
using RetailManagementSystem.Models.Models.Delivery;
using RetailManagementSystem.Models.Models.DTO;
using RetailManagementSystem.Models.Models.ResultModels;
using RetailManagementSystem.Models.Models.Retailer;
using System.Security.Cryptography.X509Certificates;
using System.Security.Claims;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using RetailManagementSystem.Utility;

namespace RetailManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApiResponse _response;
        public DeliveryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new ApiResponse();
        }

        private int InitializeDeliveryId()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email != null)
            {
                var DeliveryOwner = _unitOfWork.DeliveryUser.Get(u => u.Email == email);
                return DeliveryOwner.Id;
            }
            else
            {
                return 0;
            }
        }

        [HttpGet]
        [Authorize(Roles=SD.Role_Delivery)]
        public async Task<IActionResult> GetDeliveries()
        {
            IEnumerable<Bill> AllBills = _unitOfWork.Bill.GetAll(includeProperties: ["StoreUser"]);
            IEnumerable<Order> AllOrders = _unitOfWork.Order.GetAll(includeProperties: ["Stock.Product.SubCategory","Stock.Warehouse", "Bill"]);
            List<DeliveryDetailedResult> CombinedResult = AllBills.Select(bill =>
            {
            List<OrderFromBill> OrdersFromBill = AllOrders.Where(order => order.Bill == bill).Select(order =>
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
                IndexForBill = bill.indexForBill,
                StoreName = bill.StoreUser.StoreName,
                PhotoOfStore = bill.StoreUser.PhotoOfStore,
                ProfilePhoto = bill.StoreUser.ProfilePhoto,
                Phone = bill.StoreUser.Phone,
                Address = bill.StoreUser.Address
            };

            
            return new DeliveryDetailedResult
            {
                storeData = store,
                OrdersFromBill = OrdersFromBill
            };
            }).ToList();
            _response.Result = CombinedResult;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        [HttpPost]
        [Authorize(Roles=SD.Role_Delivery)]
        public async Task<IActionResult> CreateDelivery([FromForm] DeliveryDTO deliveryDTO)
        {
            try
            {
                var deliveryId = InitializeDeliveryId();
                if (deliveryId != 0)
                {
                    Bill billFromDb = _unitOfWork.Bill.Get(u => u.indexForBill == deliveryDTO.BillIndex);
                    Delivery delivery = new Delivery()
                    {
                        BillId = billFromDb.Id,
                        Status = deliveryDTO.Status,
                        DeliveryDate = deliveryDTO.DeliveryDate,
                        Instructions = deliveryDTO.Instructions,
                        Phone2 = deliveryDTO.Phone2,
                        GoogleMapLocation = deliveryDTO.GoogleMapLocation,

                    };
                    _unitOfWork.Delivery.Add(delivery);
                    _response.Result = delivery;
                    _response.StatusCode = HttpStatusCode.Created;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.Unauthorized;
                    return Ok(_response);
                }
                
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
            _unitOfWork.Save();
            return Ok(_response);
        }

        [HttpGet("Returns")]
        [Authorize(Roles=SD.Role_Delivery)]
        public async Task<IActionResult> GetReturn([FromQuery]string index)
        {
            Return returnObj = _unitOfWork.Return.Get(u => u.indexForReturn.ToString() == index);
            _response.Result = returnObj;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }


        [HttpPost("Returns")]
        [Authorize(Roles=SD.Role_Delivery)]
        public async Task<IActionResult> SubmitReturns([FromForm] ReturnDTO submitReturnsDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var deliveryId = InitializeDeliveryId();
                    Order orderFromDb = _unitOfWork.Order.Get(u => u.Id == submitReturnsDTO.OrderId, includeProperties: ["Stock"]);
                    var billIdFromDb = _unitOfWork.Order.Get(u => u.Id == submitReturnsDTO.OrderId).BillId;
                    //var deliveryIdFromDb = _unitOfWork.Delivery.Get(u => u.BillId == billIdFromDb).DeliveryUserId;
                    //if (deliveryId != 0 && deliveryId == deliveryIdFromDb)
                    //{
                        if (orderFromDb.Stock.IsReturnable)
                        {
                            Return returns = new Return()
                            {
                                OrderId = submitReturnsDTO.OrderId,
                                indexForReturn = Guid.NewGuid(),
                                DeliveryUserId = deliveryId,
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
                            return Ok(_response);
                        }
                        
                    //}
                    //else
                    //{
                    //    _response.IsSuccess = false;
                    //    _response.StatusCode = HttpStatusCode.Unauthorized;
                    //    return Ok(_response);
                    //}
                }
                else
                {
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
            _unitOfWork.Save();
            return Ok(_response);
        }
    }
}
