using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RetailManagementSystem.Models.Models.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.Retailer
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }
        public int IndexForDeletion { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        [ValidateNever]
        public Product Product { get; set; }
        public int WarehouseId { get; set; }
        [ForeignKey(nameof(WarehouseId))]
        [ValidateNever]
        public Warehouse Warehouse { get; set; }
        public int Quantity { get; set; }
        public float MarginPercentage { get; set; }
        public bool IsReturnable { get; set; } = true;
        public DateTime LastUpdated { get; set; }
    }
}
