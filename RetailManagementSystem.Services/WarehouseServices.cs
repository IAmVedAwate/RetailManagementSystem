﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.DTO;
using RetailManagementSystem.Models.Models.Retailer;
using RetailManagementSystem.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Services
{
    public class WarehouseServices : IWarehouseServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApiResponse _response;
        public WarehouseServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new ApiResponse();
        }

        public ApiResponse GetAllWarehousesSV(int subId)
        {
            IEnumerable<Stock> stocksFromDb = _unitOfWork.Stock.GetAll(u => u.Product.SubCategoryId == subId,
                includeProperties: ["Product.SubCategory","Warehouse"]).OrderBy(s => s.ProductId).ToList();
            _response.Result = stocksFromDb;
            _response.StatusCode = HttpStatusCode.OK;
            return (_response);
        }

        public ApiResponse GetWarehouseSV(string email)
        {
            var userId = 0;
            if (email != null)
            {
                var mainWarehouseOwner = _unitOfWork.RetailerUser.Get(u => u.Email == email);
                userId = mainWarehouseOwner?.WarehouseId ?? 0;
            }
            IEnumerable<Stock> stocksFromDb = _unitOfWork.Stock.GetAll(u => u.WarehouseId == userId, includeProperties: ["Product"]);
            _response.Result = stocksFromDb;
            _response.StatusCode = HttpStatusCode.OK;
            return (_response);
        }

        public ApiResponse AddStockInWarehouseSV(StockDTO addStockDTO, string email)
        {
            try
            {
                // Create Warehouse Object Here
                var userId = 0;
                if (email != null)
                {
                    var mainWarehouseOwner = _unitOfWork.RetailerUser.Get(u => u.Email == email);
                    userId = mainWarehouseOwner?.WarehouseId ?? 0;
                }
                Stock stockFromDb = new();

                stockFromDb.ProductId = addStockDTO.ProductId;
                stockFromDb.Quantity = addStockDTO.Quantity;
                stockFromDb.MarginPercentage = addStockDTO.MarginPercentage;
                stockFromDb.IsReturnable = addStockDTO.IsReturnable;
                stockFromDb.LastUpdated = DateTime.Now;
                stockFromDb.WarehouseId = userId;

                _unitOfWork.Stock.Add(stockFromDb);
                _response.Result = stockFromDb;
                _response.StatusCode = HttpStatusCode.Created;
                
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return (_response);
            }
            _unitOfWork.Save();
            return (_response);
        }

        public ApiResponse EditStockInWarehouseSV(int indexId, StockDTO editWarehouseDTO, string email)
        {
            try
            {
                var userId = 0;
                if (email != null)
                {
                    var mainWarehouseOwner = _unitOfWork.RetailerUser.Get(u => u.Email == email);
                    userId = mainWarehouseOwner?.WarehouseId ?? 0;
                }
                Stock stockFromDb = _unitOfWork.Stock.Get(u => u.WarehouseId == userId && u.Id == indexId);

                stockFromDb.ProductId = editWarehouseDTO.ProductId;
                stockFromDb.Quantity = editWarehouseDTO.Quantity;
                stockFromDb.MarginPercentage = editWarehouseDTO.MarginPercentage;
                stockFromDb.IsReturnable = editWarehouseDTO.IsReturnable;
                stockFromDb.LastUpdated = DateTime.Now;
                stockFromDb.WarehouseId = userId;

                _unitOfWork.Stock.Update(stockFromDb);

                _response.Result = editWarehouseDTO;
                _response.StatusCode = HttpStatusCode.Created;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return (_response);
            }
            _unitOfWork.Save();
            return (_response);
        }

        public ApiResponse RemoveStockSV(int indexId, string email)
        {
            var userId = 0;
            if (email != null)
            {
                var mainWarehouseOwner = _unitOfWork.RetailerUser.Get(u => u.Email == email);
                userId = mainWarehouseOwner?.WarehouseId ?? 0;
            }
            Stock stockForDeletion = _unitOfWork.Stock.Get(u => u.WarehouseId == userId && u.Id == indexId);
            _unitOfWork.Stock.Remove(stockForDeletion);
            _unitOfWork.Save();
            _response.Result = stockForDeletion;
            _response.StatusCode = HttpStatusCode.OK;
            return (_response);
        }
    }
}
