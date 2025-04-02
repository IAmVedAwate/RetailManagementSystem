using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.Retailer
{
    public class RetailerUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string RetailName { get; set; }
        [Required]
        public string OwnerName { get; set; }
        [Required]
        public string GSTNumber { get; set; }
        public string ProfilePhoto { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Address { get; set; }
        public int WarehouseId { get; set; }
        [ForeignKey(nameof(WarehouseId))]
        [ValidateNever]
        public Warehouse Warehouse { get; set; } // RetailerId
    }
}
