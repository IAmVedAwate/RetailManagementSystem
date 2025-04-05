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
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Services
{
    public class AdvertisementServices : IAdvertisementServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApiResponse _response;
        public AdvertisementServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new ApiResponse();
        }
        public ApiResponse GetAdvertisementsSV()
        {
            _response.Result = _unitOfWork.Advertisement.GetAll();
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }
        public ApiResponse CreateAdvertisementSV(AdvertisementDTO createAdvertisementDTO, int userId)
        {
            try
            {
                Advertisement advertisement = new Advertisement()
                {
                    AdContent = createAdvertisementDTO.AdContent,
                    TargetAudience = createAdvertisementDTO.TargetAudience,
                    DatePosted = createAdvertisementDTO.DatePosted,
                    DateExpiry = createAdvertisementDTO.DateExpiry,
                    AdLocation = createAdvertisementDTO.AdLocation,
                    AdminUserId = userId
                };
                _unitOfWork.Advertisement.Add(advertisement);
                _response.Result = advertisement;
                _response.StatusCode = HttpStatusCode.Created;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return _response;
            }
            _unitOfWork.Save();
            return _response;
        }
        public ApiResponse UpdateAdvertisementSV(int id, AdvertisementDTO updateAdvertisementDTO)
        {
            try
            {
                Advertisement advertisement = _unitOfWork.Advertisement.Get(u => u.Id == id);
                advertisement.AdContent = updateAdvertisementDTO.AdContent;
                advertisement.TargetAudience = updateAdvertisementDTO.TargetAudience;
                advertisement.DatePosted = updateAdvertisementDTO.DatePosted;
                advertisement.DateExpiry = updateAdvertisementDTO.DateExpiry;
                advertisement.AdLocation = updateAdvertisementDTO.AdLocation;
                _unitOfWork.Advertisement.Update(advertisement);
                _response.Result = updateAdvertisementDTO;
                _response.StatusCode = HttpStatusCode.Accepted;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return _response;
            }
            _unitOfWork.Save();
            return _response;
        }
        public ApiResponse DeleteAdvertisementSV(int id)
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
                return _response;
            }
            _unitOfWork.Save();
            return _response;
        }

        
    }
}
