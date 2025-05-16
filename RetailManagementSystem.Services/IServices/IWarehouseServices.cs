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
    public interface IWarehouseServices
    {
        public ApiResponse GetAllWarehousesSV(int subId);
        public ApiResponse GetWarehouseSV(string email);
        public ApiResponse GetMessagesSV(string email);
        public ApiResponse AddStockInWarehouseSV(StockDTO addStockDTO, string email);
        public ApiResponse EditStockInWarehouseSV(int indexId, StockDTO editWarehouseDTO, string email);
        public ApiResponse RemoveStockSV(int indexId, string email);
        public ApiResponse GetBillCopyForRetailerSV(string email);
    }
}
