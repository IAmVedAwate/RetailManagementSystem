using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.Delivery
{
    public class OrderAssign
    {
        [Key]
        public int Id { get; set; }
        public int DeliveryUserId { get; set; }
        [ForeignKey(nameof(DeliveryUserId))] //For Delivery User
        [ValidateNever]
        public DeliveryUser DeliveryUser { get; set; }
        public int DeliveryId { get; set; }
        [ForeignKey(nameof(DeliveryId))] //For Delivery Order
        [ValidateNever]
        public Delivery Delivery { get; set; }
    }
}
