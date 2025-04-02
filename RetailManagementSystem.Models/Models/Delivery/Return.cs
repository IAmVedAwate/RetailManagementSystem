using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.Retailer;
using RetailManagementSystem.Models.Models.Store;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.Delivery
{
    public class Return
    {
        [Key] 
        public int Id { get; set; }
        public Guid indexForReturn { get; set; }
        public int OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        [ValidateNever]
        public Order Order { get; set; }
        public int DeliveryUserId { get; set; }
        [ForeignKey(nameof(DeliveryUserId))] //DeliveryId
        [ValidateNever]
        public DeliveryUser DeliveryUser { get; set; }
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
