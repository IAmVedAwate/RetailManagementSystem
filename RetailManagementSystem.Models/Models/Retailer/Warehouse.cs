using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RetailManagementSystem.Models.Models.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.Retailer
{
    public class Warehouse
    {
        [Key]
        public int Id { get; set; }
        public int ProductsCounts { get; set; } = 0;
        public string WarehouseName { get; set; }
    }
}
