using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.DTO;
using RetailManagementSystem.Services.IServices;
using RetailManagementSystem.Utility;
using System.Net;
using System.Security.Claims;

namespace RetailManagementSystem.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AdvertisementController : ControllerBase
    {
        private IUnitOfServices _unitOfServices;
        private ApiResponse _response;
        public AdvertisementController(IUnitOfServices unitOfServices)
        {
            _unitOfServices = unitOfServices;
            _response = new ApiResponse();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAdvertisements()
        {
            ApiResponse result = _unitOfServices.AdvertisementService.GetAdvertisementsSV();
            return Ok(result);
        }
        [HttpPost]
        [Authorize(Roles=SD.Role_Admin)]
        public async Task<IActionResult> CreateAdvertisement([FromForm] AdvertisementDTO createAdvertisementDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse result = _unitOfServices.AdvertisementService.CreateAdvertisementSV(createAdvertisementDTO, Convert.ToString(User.FindFirstValue(ClaimTypes.Email)));
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
        [HttpPut("{id:int}", Name = "UpdateAdvertisement")]
        [Authorize(Roles=SD.Role_Admin)]
        public async Task<IActionResult> UpdateAdvertisement(int id,[FromForm] AdvertisementDTO updateAdvertisementDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse result = _unitOfServices.AdvertisementService.UpdateAdvertisementSV(id, updateAdvertisementDTO);
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
        [HttpDelete("{id:int}")]
        [Authorize(Roles=SD.Role_Admin)]
        public async Task<IActionResult> DeleteAdvertisement(int id)
        {
            try
            {
                if (id != null || id != 0)
                {
                    ApiResponse result = _unitOfServices.AdvertisementService.DeleteAdvertisementSV(id);
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
