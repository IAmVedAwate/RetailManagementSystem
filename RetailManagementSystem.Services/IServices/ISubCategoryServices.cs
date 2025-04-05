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
    public interface ISubCategoryServices
    {
        public ApiResponse GetSubCategoriesSV();
        public ApiResponse CreateSubCategorySV(SubCategoryDTO createSubCategoryDTO);
        public ApiResponse UpdateSubCategorySV(int id, SubCategoryDTO updateSubCategoryDTO);
        public ApiResponse DeleteSubCategorySV(int id);
    }
}
