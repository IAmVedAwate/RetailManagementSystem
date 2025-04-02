using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.DTO
{
    public class RegisterRequestDTO
    {
        [Required]
        public string Role { get; set; }
        [AllowNull]
        public string StoreName { get; set; }
        [AllowNull]
        public string AdminName { get; set; }
        [AllowNull]
        public string RetailName { get; set; }
        [AllowNull]
        public string DeliveryUserName { get; set; }
        [AllowNull]
        public string OwnerName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Username { get; set; }
        [AllowNull]
        public string Email { get; set; }
        [AllowNull]
        public string Address { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
        [AllowNull]
        public string Adhar { get; set; }
        [AllowNull]
        public string GSTNumber { get; set; }
        [AllowNull]
        public string ProfilePhoto { get; set; }
        [AllowNull]
        public string PhotoOfStore { get; set; }
        [AllowNull]
        [ValidateNever]
        public DateTime DateJoined { get; set; }


    }
}
