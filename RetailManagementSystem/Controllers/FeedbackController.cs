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

namespace RetailManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApiResponse _response;
        public FeedbackController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new ApiResponse();
        }
        [HttpGet]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> GetFeedbacks()
        {
            _response.Result = _unitOfWork.Feedback.GetAll(includeProperties: ["ApplicationUser"]);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
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
                    // Create Feedback Object Here
                    Feedback feedback = new Feedback();
                    feedback.DateSubmitted = DateTime.Now;
                    feedback.FeedbackType = submitFeedbackDTO.FeedbackType;
                    feedback.FeedbackContent = submitFeedbackDTO.FeedbackContent;
                    feedback.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    _unitOfWork.Feedback.Add(feedback);
                    _response.Result = feedback;
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
    }
}
