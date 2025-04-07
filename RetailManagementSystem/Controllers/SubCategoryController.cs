using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.DTO;
using RetailManagementSystem.Utility;
using RetailManagementSystem.Services;
using System.Net;
using RetailManagementSystem.Services.IServices;

namespace RetailManagementSystem.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private ApiResponse _response;
        private IUnitOfServices _unitOfServices;
        public SubCategoryController(IUnitOfServices unitOfServices)
        {
            _response = new ApiResponse();
            _unitOfServices = unitOfServices;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetSubCategories()
        {
            ApiResponse result = _unitOfServices.SubCategoryService.GetSubCategoriesSV();
            return Ok(result);
        }
        [HttpPost]
        [Authorize(Roles=SD.Role_Admin)]

        public async Task<IActionResult> CreateSubCategory([FromForm] SubCategoryDTO createSubCategoryDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse result = _unitOfServices.SubCategoryService.CreateSubCategorySV(createSubCategoryDTO);
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

        [HttpPut("{id:int}")]
        [Authorize(Roles=SD.Role_Admin)]
        public async Task<IActionResult> UpdateSubCategory(int id, [FromForm] SubCategoryDTO updateSubCategoryDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse result = _unitOfServices.SubCategoryService.UpdateSubCategorySV(id , updateSubCategoryDTO);
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
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            try
            {
                if (id!=null || id!=0)
                {
                    ApiResponse result = _unitOfServices.SubCategoryService.DeleteSubCategorySV(id);
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
