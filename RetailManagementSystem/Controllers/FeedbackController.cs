using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.Store;
using RetailManagementSystem.Models.Models;
using System.Net;
using RetailManagementSystem.Models.Models.DTO;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RetailManagementSystem.Utility;
using RetailManagementSystem.Services.IServices;

namespace RetailManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IUnitOfServices _unitOfServices;
        private ApiResponse _response;
        public FeedbackController(IUnitOfServices unitOfServices)
        {
            _unitOfServices = unitOfServices;
            _response = new ApiResponse();
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFeedbacks()
        {
            ApiResponse result = _unitOfServices.FeedbackService.GetFeedbacksSV();
            return Ok(result);
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Feedback>> SubmitFeedback([FromForm] FeedbackDTO submitFeedbackDTO)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (ModelState.IsValid)
                {
                    ApiResponse result = _unitOfServices.FeedbackService.SubmitFeedbackSV(submitFeedbackDTO, userId);
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
