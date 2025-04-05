using Microsoft.AspNetCore.Mvc;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.DTO;
using RetailManagementSystem.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Services
{
    public class FeedbackServices : IFeedbackServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApiResponse _response;
        public FeedbackServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new ApiResponse();
        }

        public ApiResponse GetFeedbacksSV()
        {
            _response.Result = _unitOfWork.Feedback.GetAll(includeProperties: ["ApplicationUser"]);
            _response.StatusCode = HttpStatusCode.OK;
            return (_response);
        }

        public ApiResponse SubmitFeedbackSV(FeedbackDTO submitFeedbackDTO, string userId)
        {
            try
            {
                    // Create Feedback Object Here
                    Feedback feedback = new Feedback();
                    feedback.DateSubmitted = DateTime.Now;
                    feedback.FeedbackType = submitFeedbackDTO.FeedbackType;
                    feedback.FeedbackContent = submitFeedbackDTO.FeedbackContent;
                    feedback.ApplicationUserId = userId;
                    _unitOfWork.Feedback.Add(feedback);
                    _response.Result = feedback;
                    _response.StatusCode = HttpStatusCode.Created;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return (_response);
            }
            _unitOfWork.Save();
            return (_response);
        }
    }
}
