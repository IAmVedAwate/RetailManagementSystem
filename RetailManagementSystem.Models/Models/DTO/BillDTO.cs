using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RetailManagementSystem.Models.Models.Store;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.DTO
{
    public class BillDTO
    {
        public IEnumerable<OrderDTO> orders { get; set; }
        public float TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime DateGenerated { get; set; }
    }
}
