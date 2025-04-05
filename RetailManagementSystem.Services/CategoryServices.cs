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
    public class CategoryServices : ICategoryServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApiResponse _response;
        public CategoryServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new ApiResponse();
        }

        public ApiResponse GetCategoriesSV()
        {
            // Selecting All Categories
            //REASON: To Render them for Admin User to View.
            IEnumerable<Category> categoriesFromDb = _unitOfWork.Category.GetAll();

            _response.Result = categoriesFromDb;
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }

        public ApiResponse CreateCategorySV(CategoryDTO createCategoryDTO)
        {
            // Converting DTO into main Category
            // REASON: To save the category in Database and prevent from user let know the main structure of model.
            Category category = new Category()
            {
                CategoryName = createCategoryDTO.CategoryName,
                CategoryDescription = createCategoryDTO.CategoryDescription
            };
            _unitOfWork.Category.Add(category);
            _unitOfWork.Save();

            _response.Result = createCategoryDTO;
            _response.StatusCode = HttpStatusCode.Created;

            return _response;
        }

        public ApiResponse UpdateCategorySV(int id, CategoryDTO updateCategoryDTO)
        {
            // Converting DTO into main Category
            // REASON: To save the category in Database and prevent from user let know the main structure of model.
            Category category = _unitOfWork.Category.Get(u => u.Id == id);

            category.CategoryName = updateCategoryDTO.CategoryName;
            category.CategoryDescription = updateCategoryDTO.CategoryDescription;
            
            _unitOfWork.Category.Update(category);
            _unitOfWork.Save();

            _response.Result = updateCategoryDTO;
            _response.StatusCode = HttpStatusCode.Accepted;
            return _response;
        }

        public ApiResponse DeleteCategorySV(int id)
        {
            // Converting DTO into main Category
            // REASON: To save the category in Database and prevent from user let know the main structure of model.
            Category deletableCategory = _unitOfWork.Category.Get(u => u.Id == id);
            _unitOfWork.Category.Remove(deletableCategory);
            _unitOfWork.Save();

            _response.StatusCode = HttpStatusCode.Accepted;
            return _response;
        }

    }
}
