using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.DTO;
using RetailManagementSystem.Utility;
using System.Net;
using System.Security.Claims;

namespace RetailManagementSystem.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AdvertisementController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApiResponse _response;
        public AdvertisementController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new ApiResponse();
        }

        private int InitializeAdminId()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email != null)
            {
                var Admin = _unitOfWork.AdminUser.Get(u => u.Email == email);
                return Admin.Id;
            }
            else
            {
                return 0;
            }
        }

        [HttpGet]
        [Authorize(Roles=SD.Role_Admin)]
        public async Task<IActionResult> GetAdvertisements()
        {
            _response.Result = _unitOfWork.Advertisement.GetAll();
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        [HttpPost]
        [Authorize(Roles=SD.Role_Admin)]
        public async Task<IActionResult> CreateAdvertisement([FromForm] AdvertisementDTO createAdvertisementDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Advertisement advertisement = new Advertisement()
                    {
                        AdContent = createAdvertisementDTO.AdContent,
                        TargetAudience = createAdvertisementDTO.TargetAudience,
                        DatePosted = createAdvertisementDTO.DatePosted,
                        DateExpiry = createAdvertisementDTO.DateExpiry,
                        AdLocation = createAdvertisementDTO.AdLocation,
                        AdminUserId = InitializeAdminId()
                    };
                    _unitOfWork.Advertisement.Add(advertisement);
                    _response.Result = advertisement;
                    _response.StatusCode = HttpStatusCode.Created;
                }
                else
                {
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
            _unitOfWork.Save();
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
                    Advertisement advertisement = _unitOfWork.Advertisement.Get(u=> u.Id == id);
                    advertisement.AdContent = updateAdvertisementDTO.AdContent;
                    advertisement.TargetAudience = updateAdvertisementDTO.TargetAudience;
                    advertisement.DatePosted = updateAdvertisementDTO.DatePosted;
                    advertisement.DateExpiry = updateAdvertisementDTO.DateExpiry;
                    advertisement.AdLocation = updateAdvertisementDTO.AdLocation;
                    _unitOfWork.Advertisement.Update(advertisement);
                    _response.Result = updateAdvertisementDTO;
                    _response.StatusCode = HttpStatusCode.Accepted;
                }
                else
                {
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
            _unitOfWork.Save();
            return Ok(_response);
        }
        [HttpDelete]
        [Authorize(Roles=SD.Role_Admin)]
        public async Task<IActionResult> DeleteAdvertisement([FromBody] int id)
        {
            try
            {
                if (id != null || id != 0)
                {

                    Advertisement deletableAdvertisement = _unitOfWork.Advertisement.Get(u => u.Id == id);
                    _unitOfWork.Advertisement.Remove(deletableAdvertisement);
                    _response.StatusCode = HttpStatusCode.Accepted;
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
            _unitOfWork.Save();
            return Ok(_response);
        }
    }
}
