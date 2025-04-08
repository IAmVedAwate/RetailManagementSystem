using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.DTO;
using RetailManagementSystem.Services.IServices;
using RetailManagementSystem.Utility;
using System.Net;

namespace RetailManagementSystem.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IUnitOfServices _unitOfServices;
        private ApiResponse _response;
        public ProductController(IUnitOfServices unitOfServices)
        {
            _unitOfServices = unitOfServices;
            _response = new ApiResponse();
        }
        [HttpGet]
        [Authorize(Roles=SD.Role_Admin)]
        public async Task<IActionResult> GetProducts()
        {
            ApiResponse result = _unitOfServices.ProductService.GetProductsSV();
            return Ok(result);
        }
        [HttpGet("productById/{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetProductById(int id)
        {
            ApiResponse result = _unitOfServices.ProductService.GetProductByIdSV(id);
            return Ok(result);
        }
        [HttpGet("{subId:int}")]
        [Authorize]
        public async Task<IActionResult> GetProductBySubCategory(int subId)
        {
            ApiResponse result = _unitOfServices.ProductService.GetProductBySubCategorySV(subId);
            return Ok(result);
        }
        [HttpGet("random")]
        public async Task<IActionResult> GetRandomProducts()
        {
            ApiResponse result = _unitOfServices.ProductService.GetRandomProductsSV();
            return Ok(result);
        }
        [HttpPost]
        [Authorize(Roles=SD.Role_Admin)]
        public async Task<IActionResult> CreateProduct([FromForm] ProductDTO createProductDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse result = await _unitOfServices.ProductService.CreateProductSV(createProductDTO);
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
        [HttpPut("{id:int}", Name = "UpdateProduct")]
        [Authorize(Roles=SD.Role_Admin)]
        public async Task<IActionResult> UpdateProduct(int id,[FromForm] ProductDTO updateProductDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse result = await _unitOfServices.ProductService.UpdateProductSV(id, updateProductDTO);
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
        [HttpDelete]
        [Authorize(Roles=SD.Role_Admin)]
        public async Task<IActionResult> DeleteProduct([FromForm] int id)
        {
            try
            {
                if (id != null || id != 0)
                {
                    ApiResponse result = await _unitOfServices.ProductService.DeleteProductSV(id);
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
