using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.Retailer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.Store
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int StockId { get; set; }
        [ForeignKey(nameof(StockId))]
        [ValidateNever]
        public Stock Stock { get; set; }
        public int Quantity { get; set; }
        public float TotalAmount { get; set; }
        public int BillId { get; set; }
        [ForeignKey(nameof(BillId))]
        [ValidateNever]
        public Bill Bill { get; set; }
    }
}
