using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.DTO;
using RetailManagementSystem.Models.Models;
using System.Net;
using RetailManagementSystem.Models.Models.Store;
using System.Security.Claims;
using RetailManagementSystem.Models.Models.Retailer;
using Microsoft.AspNetCore.Authorization;
using RetailManagementSystem.Utility;

namespace RetailManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApiResponse _response;
        public BillController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new ApiResponse();
        }

        private int InitializeStoreId()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email != null)
            {
                var BillOwner = _unitOfWork.StoreUser.Get(u => u.Email == email);
                return BillOwner.Id;
            }
            else
            {
                return 0;
            }
        }

        [HttpGet]
        [Authorize(Roles=SD.Role_Store)]
        public async Task<IActionResult> GetBills()
        {
            var billOwnerId = InitializeStoreId();
            if (billOwnerId != 0)
            {
                _response.Result = _unitOfWork.Bill.GetAll(u=> u.StoreId == billOwnerId, includeProperties: ["StoreUser"]);
                _response.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                _response.StatusCode= HttpStatusCode.Unauthorized;
                _response.IsSuccess = false;
            }
            return Ok(_response);
        }
        [HttpGet("Order")]
        [Authorize(Roles=SD.Role_Store)]
        public async Task<IActionResult> GetBill([FromQuery]string index)
        {
            var billOwnerId = InitializeStoreId();
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
            return Ok(_response);
        }

        [HttpPost]
        [Authorize(Roles=SD.Role_Store)]
        public async Task<IActionResult> CreateBill([FromBody] BillDTO createBillDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Create Warehouse Object Here
                    Bill newBill = new Bill();
                    var billOwnerId = InitializeStoreId();
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
                else
                {
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
            _unitOfWork.Save();
            return Ok(_response);
        }

        [HttpPut("Order/{id:int}", Name = "UpdateBill")]
        [Authorize(Roles=SD.Role_Store)]
        public async Task<IActionResult> UpdateBill(int id, [FromForm] OrderDTO updateBillDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var billOwnerId = InitializeStoreId();
                    if(billOwnerId != 0)
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
                else
                {
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
            _unitOfWork.Save();
            return Ok(_response);
        }

        [HttpDelete("Order/{id:int}", Name = "DeleteOrder")]
        [Authorize(Roles=SD.Role_Store)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                Order orderFromDb = _unitOfWork.Order.Get(u => u.Id == id);
                IEnumerable<Order> orderEntries = _unitOfWork.Order.GetAll(u => u.BillId == orderFromDb.BillId);
                Bill billFromDb = _unitOfWork.Bill.Get(u => u.Id == orderFromDb.BillId);
                billFromDb.TotalAmount = (float)orderEntries.Select(order => order.TotalAmount).Sum() - orderFromDb.TotalAmount;

                _unitOfWork.Order.Remove(orderFromDb);
                _unitOfWork.Bill.Update(billFromDb);

                _unitOfWork.Save();
                _response.Result = "Deleted Successfully !! ";
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpDelete("{id:int}", Name = "DeleteBill")]
        [Authorize(Roles=SD.Role_Store)]
        public async Task<IActionResult> DeleteBill(int id)
        {
            Bill billFromDb = _unitOfWork.Bill.Get(u => u.Id == id);
            var billOwnerId = InitializeStoreId();
            if (billFromDb.StoreId == billOwnerId)
            {
                IEnumerable<Order> orders = _unitOfWork.Order.GetAll(u => u.BillId == billFromDb.Id);
                _unitOfWork.Order.RemoveRange(orders);
                _unitOfWork.Bill.Remove(billFromDb);
                _unitOfWork.Save();
                _response.Result = "Deleted Successfully !! ";
                _response.StatusCode = HttpStatusCode.OK;
            }
            
            return Ok(_response);
        }
    }
}
