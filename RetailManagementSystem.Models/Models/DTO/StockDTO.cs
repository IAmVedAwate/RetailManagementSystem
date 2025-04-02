using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RetailManagementSystem.Models.Models.Retailer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.DTO
{
    public class StockDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public float MarginPercentage { get; set; }
        public bool IsReturnable { get; set; } = true;
        public DateTime LastUpdated { get; set; }
    }
}
