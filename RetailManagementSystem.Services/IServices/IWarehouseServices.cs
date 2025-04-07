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
        public ApiResponse AddStockInWarehouseSV(WarehouseDTO addWarehouseDTO, string email);
        public ApiResponse EditStockInWarehouseSV(int indexId, StockDTO editWarehouseDTO, string email);
        public ApiResponse RemoveStockSV(int indexId, string email);
    }
}
