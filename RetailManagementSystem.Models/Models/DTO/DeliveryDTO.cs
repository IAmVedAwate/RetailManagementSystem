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
    public class DeliveryDTO
    {
        public string BillIndex { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime DeliveryDate { get; set; }
        public string Instructions { get; set; }
        public string Phone2 { get; set; }
        public string GoogleMapLocation { get; set; }
    }
}
