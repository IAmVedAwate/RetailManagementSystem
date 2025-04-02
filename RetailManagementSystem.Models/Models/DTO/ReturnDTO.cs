using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RetailManagementSystem.Models.Models.Delivery;
using RetailManagementSystem.Models.Models.Store;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.DTO
{
    public class ReturnDTO
    {
        public int OrderId { get; set; }
        public string ReturnReason { get; set; }
        public string ReturnStatus { get; set; } = "Pending";
        public DateTime ApprovalDate { get; set; }
        public float RefundAmmount { get; set; }
        public string Comments { get; set; }
        public bool IsReturnable { get; set; } = true;
        public string ReturnMethod { get; set; }
        public string PhotoEvidence { get; set; }
    }
}
