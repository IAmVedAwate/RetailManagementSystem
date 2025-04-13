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
using RetailManagementSystem.Services.IServices;
using Microsoft.AspNetCore.Http.HttpResults;

namespace RetailManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private IUnitOfServices _unitOfServices;
        private ApiResponse _response;
        public DeliveryController(IUnitOfServices unitOfServices)
        {
            _unitOfServices = unitOfServices;
            _response = new ApiResponse();
        }
        

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetDeliveries()
        {
            ApiResponse result = _unitOfServices.DeliveryService.GetDeliveriesSV();
            return Ok(result);
        }
        [HttpPost]
        [Authorize(Roles=SD.Role_Store)]
        public async Task<IActionResult> CreateDelivery([FromBody] DeliveryDTO deliveryDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse result = _unitOfServices.DeliveryService.CreateDeliverySV(deliveryDTO, Convert.ToString(User.FindFirstValue(ClaimTypes.Email)));
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
        [HttpPost("{id:int}")]
        [Authorize(Roles = SD.Role_Delivery)]
        public async Task<IActionResult> AssignDeliveryToUser(int id){
            ApiResponse result = _unitOfServices.DeliveryService.AssignDeliveryToUserSV(id, Convert.ToString(User.FindFirstValue(ClaimTypes.Email)));
            return Ok(result);
        }
        [HttpGet("{id:int}")]
        [Authorize(Roles = SD.Role_Delivery)]
        public async Task<IActionResult> AssignedDeliveryById(int id)
        {
            ApiResponse result = _unitOfServices.DeliveryService.AssignedDeliveryByIdSV(id, Convert.ToString(User.FindFirstValue(ClaimTypes.Email)));
            return Ok(result);
        }
        [HttpGet("Returns")]
        [Authorize(Roles=SD.Role_Delivery)]
        public async Task<IActionResult> GetReturn([FromQuery]string index)
        {
            ApiResponse result = _unitOfServices.DeliveryService.GetReturnSV(index);
            return Ok(result);
        }

        [HttpPost("Returns")]
        [Authorize(Roles=SD.Role_Delivery)]
        public async Task<IActionResult> SubmitReturns([FromForm] ReturnDTO submitReturnsDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse result = _unitOfServices.DeliveryService.SubmitReturnsSV(submitReturnsDTO, Convert.ToString(User.FindFirstValue(ClaimTypes.Email)));
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
