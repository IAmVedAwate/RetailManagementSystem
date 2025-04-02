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
        private readonly IUnitOfWork _unitOfWork;
        private ApiResponse _response;
        private IWebHostEnvironment _webHostEnvironment;
        private IBlobService _blobService;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IBlobService blobService)
        {
            _unitOfWork = unitOfWork;
            _response = new ApiResponse();
            _webHostEnvironment = webHostEnvironment;
            _blobService = blobService;
        }
        [HttpGet]
        [Authorize(Roles=SD.Role_Admin)]
        public async Task<IActionResult> GetProducts()
        {
            _response.Result = _unitOfWork.Product.GetAll(includeProperties: ["Category"]);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        [HttpPost]
        [Authorize(Roles=SD.Role_Admin)]
        public async Task<IActionResult> CreateProduct([FromForm] ProductDTO createProductDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var categoryExists = _unitOfWork.Category.Get(u=> u.Id == createProductDTO.CategoryId);
                    if (categoryExists==null)
                    {
                        _response.IsSuccess = false;
                        _response.ErrorMessages = new List<string>() { "Invalid CategoryId" };
                        return BadRequest(_response);
                    }
                    if (createProductDTO.File == null || createProductDTO.File.Length == 0)
                    {
                        return BadRequest();
                    }
                    
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(createProductDTO.File.FileName)}";
                    // Upload the new image
                    Product product = new Product()
                    {
                        ProductName = createProductDTO.ProductName,
                        ProductDescription = createProductDTO.ProductDescription,
                        RetailPrice = createProductDTO.RetailPrice,
                        MRP = createProductDTO.MRP,
                        QuantityInBox = createProductDTO.QuantityInBox,
                        IsFamous = createProductDTO.IsFamous,
                        IsRecommended = createProductDTO.IsRecommended,
                        IsReplaceable = createProductDTO.IsReplaceable,
                        CategoryId = createProductDTO.CategoryId,
                        Image = await _blobService.UploadBlob(fileName, SD.Storage_Container, createProductDTO.File)
                    };
                    _unitOfWork.Product.Add(product);
                    _response.Result = product;
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
        [HttpPut("{id:int}", Name = "UpdateProduct")]
        [Authorize(Roles=SD.Role_Admin)]
        public async Task<IActionResult> UpdateProduct(int id,[FromForm] ProductDTO updateProductDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (updateProductDTO.File == null || updateProductDTO.File.Length == 0)
                    {
                        return BadRequest();
                    }
                    var oldFileName = _unitOfWork.Product.Get(u=>u.Id == id).Image.ToString();
                    var existingBlob = await _blobService.GetBlob(oldFileName, SD.Storage_Container);

                    if (existingBlob != null)
                    {
                        // Delete the existing blob (old image)
                        await _blobService.DeleteBlob(oldFileName.Split("/").Last(), SD.Storage_Container);
                    }
                    Product product = _unitOfWork.Product.Get(u=>u.Id == id);
                    product.ProductName = updateProductDTO.ProductName;
                    product.ProductDescription = updateProductDTO.ProductDescription;
                    product.RetailPrice = updateProductDTO.RetailPrice;
                    product.MRP = updateProductDTO.MRP;
                    product.QuantityInBox = updateProductDTO.QuantityInBox;
                    product.IsFamous = updateProductDTO.IsFamous;
                    product.IsRecommended = updateProductDTO.IsRecommended;
                    product.IsReplaceable = updateProductDTO.IsReplaceable;
                    product.CategoryId = updateProductDTO.CategoryId;

                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(updateProductDTO.File.FileName)}";
                    product.Image = await _blobService.UploadBlob(fileName, SD.Storage_Container, updateProductDTO.File);

                    _unitOfWork.Product.Update(product);
                    _response.Result = product;
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
        [HttpDelete]
        [Authorize(Roles=SD.Role_Admin)]
        public async Task<IActionResult> DeleteProduct([FromForm] int id)
        {
            try
            {
                if (id != null || id != 0)
                {

                    Product deletableProduct = _unitOfWork.Product.Get(u => u.Id == id);
                    await _blobService.DeleteBlob(deletableProduct.Image.Split("/").Last(), SD.Storage_Container);
                    _unitOfWork.Product.Remove(deletableProduct);
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
