using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RetailManagementSystem.Models.Models.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.DTO
{
    public class ProductDTO
    {
        [Required]
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int RetailPrice { get; set; }
        public int MRP { get; set; }
        public bool IsReplaceable { get; set; }
        public bool IsRecommended { get; set; }
        public bool IsFamous { get; set; }
        [Required]
        public int SubCategoryId { get; set; }
        public IFormFile File { get; set; }
    }
}
