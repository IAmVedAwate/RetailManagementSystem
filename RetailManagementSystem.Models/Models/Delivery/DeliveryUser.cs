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
    public class DeliveryUser
    {
        [Key] 
        public int Id { get; set; }
        [Required]
        public string DeliveryUserName { get; set; }
        [Required]
        public string Adhar { get; set; }
        [Required]
        public string ProfilePhoto { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Address { get; set; }

    }
}
