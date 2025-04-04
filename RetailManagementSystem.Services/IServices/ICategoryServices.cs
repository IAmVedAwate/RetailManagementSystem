﻿using RetailManagementSystem.Models.Models;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Services.IServices
{
    public interface ICategoryServices
    {
        public ApiResponse GetCategoriesSV();
        public ApiResponse CreateCategorySV(CategoryDTO createCategoryDTO);
        public ApiResponse UpdateCategorySV(int id, CategoryDTO updateCategoryDTO);
        public ApiResponse DeleteCategorySV(int id);
    }
}
