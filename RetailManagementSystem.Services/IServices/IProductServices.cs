using RetailManagementSystem.Models.Models;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Services.IServices
{
    public interface IProductServices
    {
        public ApiResponse GetProductsSV();
        public ApiResponse GetProductBySubCategorySV(int subId);
        public ApiResponse GetRandomProductsSV();
        public ApiResponse GetProductByIdSV(int id);
        public Task<ApiResponse> CreateProductSV(ProductDTO createProductDTO);
        public Task<ApiResponse> UpdateProductSV(int id, ProductDTO updateProductDTO);
        public Task<ApiResponse> DeleteProductSV(int id);
    }
}
