using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.DTO
{
    public class AdvertisementDTO
    {
        [Required]
        public string AdContent { get; set; }
        public string TargetAudience { get; set; }
        public DateTime DatePosted { get; set; }
        public DateTime DateExpiry { get; set; }
        public string AdLocation { get; set; }
    }
}
