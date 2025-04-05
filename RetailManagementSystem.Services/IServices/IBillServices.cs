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
        public ApiResponse GetBillsSV(int userId);
        public ApiResponse GetBillSV(string index, int userId);
        public ApiResponse CreateBillSV(BillDTO createBillDTO, int userId);
        public ApiResponse UpdateBillSV(int id, OrderDTO updateBillDTO, int userId);
        public ApiResponse DeleteOrderSV(int id, int userId);
        public ApiResponse DeleteBillSV(int id, int userId);
    }
}
