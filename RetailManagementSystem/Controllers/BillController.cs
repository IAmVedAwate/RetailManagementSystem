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
using RetailManagementSystem.Services.IServices;

namespace RetailManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private IUnitOfServices _unitOfServices;
        private ApiResponse _response;
        public BillController(IUnitOfServices unitOfServices)
        {
            _unitOfServices = unitOfServices;
            _response = new ApiResponse();
        }

        [HttpGet]
        [Authorize(Roles=SD.Role_Store)]
        public async Task<IActionResult> GetBills()
        {
            ApiResponse result = _unitOfServices.BillService.GetBillsSV(Convert.ToString(User.FindFirstValue(ClaimTypes.Email)));
            return Ok(result);
        }
        [HttpGet("Order")]
        [Authorize]
        public async Task<IActionResult> GetBill([FromQuery]string index)
        {
            ApiResponse result = _unitOfServices.BillService.GetBillSV(index,Convert.ToString(User.FindFirstValue(ClaimTypes.Email)));
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles=SD.Role_Store)]
        public async Task<IActionResult> CreateBill([FromBody] BillDTO createBillDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse result = _unitOfServices.BillService.CreateBillSV(createBillDTO, Convert.ToString(User.FindFirstValue(ClaimTypes.Email)));
                    _response = result;
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
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
                    ApiResponse result = _unitOfServices.BillService.UpdateBillSV(id, updateBillDTO, Convert.ToString(User.FindFirstValue(ClaimTypes.Email)));
                    _response = result;
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpDelete("Order/{id:int}", Name = "DeleteOrder")]
        [Authorize(Roles=SD.Role_Store)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                if (id != null || id != 0)
                {
                    ApiResponse result = _unitOfServices.BillService.DeleteOrderSV(id, Convert.ToString(User.FindFirstValue(ClaimTypes.Email)));
                    _response = result;
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
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
            try
            {
                if (id != null || id != 0)
                {
                    ApiResponse result = _unitOfServices.BillService.DeleteBillSV(id, Convert.ToString(User.FindFirstValue(ClaimTypes.Email)));
                    _response = result;
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
            return Ok(_response);
        }
    }
}
