using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.Store
{
    public class Bill
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string indexForBill { get; set; } = Guid.NewGuid().ToString();
        public int StoreId { get; set; }
        [ForeignKey(nameof(StoreId))]
        [ValidateNever]
        public StoreUser StoreUser { get; set; }
        public float TotalAmount { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public DateTime DateGenerated { get; set; }
        public string billName { get; set; }
    }
}
