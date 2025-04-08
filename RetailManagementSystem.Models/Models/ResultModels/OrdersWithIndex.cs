using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RetailManagementSystem.Models.Models.Retailer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.ResultModels
{
    public class OrdersWithIndex
    {
        public int StockId { get; set; }
        public int Quantity { get; set; }
        public float TotalAmount { get; set; }
        public int BillId { get; set; }
        public string BillIndex { get; set; }
    }
}
