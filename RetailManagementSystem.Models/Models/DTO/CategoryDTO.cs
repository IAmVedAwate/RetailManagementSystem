using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.DTO
{
    public class CategoryDTO
    {
        [Required]
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; } = string.Empty;
    }
}
