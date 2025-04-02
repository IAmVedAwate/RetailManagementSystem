using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.Admin
{
    public class Advertisement
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string AdContent { get; set; }
        public int AdminUserId { get; set; }
        [ForeignKey(nameof(AdminUserId))]
        [ValidateNever]
        public AdminUser AdminUser { get; set; }
        public string TargetAudience { get; set; }
        public DateTime DatePosted { get; set; }
        public DateTime DateExpiry { get; set; }
        public string AdLocation { get; set; }
    }
}
