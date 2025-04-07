using Microsoft.AspNetCore.Mvc;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.DTO;
using RetailManagementSystem.Services.IServices;
using RetailManagementSystem.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Services
{
    public class ProductServices : IProductServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private IBlobService _blobService;
        private ApiResponse _response;
        public ProductServices(IUnitOfWork unitOfWork, IBlobService blobService)
        {
            _unitOfWork = unitOfWork;
            _blobService = blobService;
            _response = new ApiResponse();
        }
        public ApiResponse GetProductsSV()
        {
            try {
                _response.Result = _unitOfWork.Product.GetAll(includeProperties: ["SubCategory"]);
                _response.StatusCode = HttpStatusCode.OK;
                return (_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return (_response);
            }
        }
        public ApiResponse GetProductByIdSV(int id)
        {
            try
            {
                _response.Result = _unitOfWork.Product.Get(u=> u.Id==id,includeProperties: ["SubCategory"]);
                _response.StatusCode = HttpStatusCode.OK;
                return (_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return (_response);
            }
        }

        public ApiResponse GetRandomProductsSV()
        {
            try
            {
                int count = _unitOfWork.SubCategory.CountAll();
                if (count == null || count== 0)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string> { "No subcategories found." };
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return _response;
                }

                // Create a random index (0-based index)
                Random random = new Random();
                int randomIndex = random.Next(1, count);  // Generates a number between 1 and count

                // Retrieve products that belong to the randomly selected subcategory
                IEnumerable<Product> products = _unitOfWork.Product.GetAll(p => p.SubCategoryId == randomIndex, includeProperties: ["SubCategory"]);

                _response.Result = products;
                _response.StatusCode = HttpStatusCode.OK;
                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return _response;
            }
        }
        public async Task<ApiResponse> CreateProductSV(ProductDTO createProductDTO)
        {
            try
            {
                
                var categoryExists = _unitOfWork.SubCategory.Get(u => u.Id == createProductDTO.SubCategoryId);
                if (categoryExists == null)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Invalid SubCategoryId" };
                    return (_response);
                }
                if (createProductDTO.File == null || createProductDTO.File.Length == 0)
                {
                    _response.IsSuccess = false;
                    return (_response);
                }

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(createProductDTO.File.FileName)}";
                // Upload the new image
                Product product = new Product()
                {
                    ProductName = createProductDTO.ProductName,
                    ProductDescription = createProductDTO.ProductDescription,
                    RetailPrice = createProductDTO.RetailPrice,
                    MRP = createProductDTO.MRP,
                    IsFamous = createProductDTO.IsFamous,
                    IsRecommended = createProductDTO.IsRecommended,
                    IsReplaceable = createProductDTO.IsReplaceable,
                    SubCategoryId = createProductDTO.SubCategoryId,
                    Image = await _blobService.UploadBlob(fileName, SD.Storage_Container, createProductDTO.File)
                };
                _unitOfWork.Product.Add(product);
                _response.Result = product;
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
        public async Task<ApiResponse> UpdateProductSV(int id, ProductDTO updateProductDTO)
        {
            try
            {
                if (updateProductDTO.File == null || updateProductDTO.File.Length == 0)
                {
                    _response.IsSuccess = false;
                    return _response;
                }
                var oldFileName = _unitOfWork.Product.Get(u => u.Id == id).Image.ToString();
                var existingBlob = await _blobService.GetBlob(oldFileName, SD.Storage_Container);

                if (existingBlob != null)
                {
                    // Delete the existing blob (old image)
                    await _blobService.DeleteBlob(oldFileName.Split("/").Last(), SD.Storage_Container);
                }
                Product product = _unitOfWork.Product.Get(u => u.Id == id);
                product.ProductName = updateProductDTO.ProductName;
                product.ProductDescription = updateProductDTO.ProductDescription;
                product.RetailPrice = updateProductDTO.RetailPrice;
                product.MRP = updateProductDTO.MRP;
                product.IsFamous = updateProductDTO.IsFamous;
                product.IsRecommended = updateProductDTO.IsRecommended;
                product.IsReplaceable = updateProductDTO.IsReplaceable;
                product.SubCategoryId = updateProductDTO.SubCategoryId;

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(updateProductDTO.File.FileName)}";
                product.Image = await _blobService.UploadBlob(fileName, SD.Storage_Container, updateProductDTO.File);

                _unitOfWork.Product.Update(product);
                _response.Result = product;
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
        public async Task<ApiResponse> DeleteProductSV(int id)
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
                return (_response);
            }
            _unitOfWork.Save();
            return (_response);
        }

    }
}
