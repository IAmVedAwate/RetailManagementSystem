using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.DTO
{
    public class SubCategoryDTO
    {
        [Required]
        public string SubCategoryName { get; set; }
        public int CategoryId { get; set; }
    }
}
