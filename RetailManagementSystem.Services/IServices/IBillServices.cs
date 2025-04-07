using Microsoft.AspNetCore.Mvc;
using RetailManagementSystem.Models.Models;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Services.IServices
{
    public interface IBillServices
    {
        public ApiResponse GetBillsSV(string email);
        public ApiResponse GetBillSV(string index, string email);
        public ApiResponse CreateBillSV(BillDTO createBillDTO, string email);
        public ApiResponse UpdateBillSV(int id, OrderDTO updateBillDTO, string email);
        public ApiResponse DeleteOrderSV(int id, string email);
        public ApiResponse DeleteBillSV(int id, string email);
    }
}
