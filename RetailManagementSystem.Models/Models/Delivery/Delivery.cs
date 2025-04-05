using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
    public class Delivery
    {
        [Key]
        public int Id { get; set; }
        public int BillId { get; set; }
        [ForeignKey(nameof(BillId))]
        [ValidateNever]
        public Bill Bill { get; set; }
        public string Status { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Instructions { get; set; }
        public string Phone2 { get; set; }
        public string GoogleMapLocation { get; set; }
    }
}
