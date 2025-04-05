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
        public ApiResponse GetWarehouseSV(int userId);
        public ApiResponse AddStockInWarehouseSV(WarehouseDTO addWarehouseDTO, int userId);
        public ApiResponse EditStockInWarehouseSV(int indexId, StockDTO editWarehouseDTO, int userId);
        public ApiResponse RemoveStockSV(int indexId, int userId);
    }
}
