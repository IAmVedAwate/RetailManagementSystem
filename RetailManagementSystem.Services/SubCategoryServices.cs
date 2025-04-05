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
    public class SubCategoryServices : ISubCategoryServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApiResponse _response;
        public SubCategoryServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new ApiResponse();
        }
        public ApiResponse GetSubCategoriesSV()
        {
           
            try
            {
                _response.Result = _unitOfWork.SubCategory.GetAll(includeProperties: ["Category"]);
                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return (_response);
            }
        }
        public ApiResponse CreateSubCategorySV(SubCategoryDTO createSubCategoryDTO)
        {
            try
            {
                SubCategory subCategory = new();
                subCategory.CategoryId = createSubCategoryDTO.CategoryId;
                subCategory.SubCategoryName = createSubCategoryDTO.SubCategoryName;
                _unitOfWork.SubCategory.Add(subCategory);
                _unitOfWork.Save();
                _response.Result = createSubCategoryDTO;
                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return (_response);
            }
        }
        public ApiResponse UpdateSubCategorySV(int id, SubCategoryDTO updateSubCategoryDTO)
        {
            try
            {
                SubCategory subCategory = _unitOfWork.SubCategory.Get(u => u.Id == id);
                subCategory.CategoryId = updateSubCategoryDTO.CategoryId;
                subCategory.SubCategoryName = updateSubCategoryDTO.SubCategoryName;
                _unitOfWork.SubCategory.Update(subCategory);
                _unitOfWork.Save();
                _response.Result = updateSubCategoryDTO;
                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return (_response);
            }
        }
        public ApiResponse DeleteSubCategorySV(int id)
        {
            SubCategory deletableSubCategory = _unitOfWork.SubCategory.Get(u => u.Id == id);
            _unitOfWork.SubCategory.Remove(deletableSubCategory);
            _unitOfWork.Save();

            _response.StatusCode = HttpStatusCode.Accepted;
            return _response;
        }

        
    }
}
