using Microsoft.AspNetCore.Mvc;
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

        public ApiResponse GetWarehouseSV(int userId)
        {
            IEnumerable<Stock> stocksFromDb = _unitOfWork.Stock.GetAll(u => u.WarehouseId == userId, includeProperties: ["Product"]);
            _response.Result = stocksFromDb.Select(stock => new Stock
            {
                ProductId = stock.ProductId,
                Product = stock.Product,
                Quantity = stock.Quantity,
                MarginPercentage = stock.MarginPercentage,
                IsReturnable = stock.IsReturnable,
                LastUpdated = stock.LastUpdated,
                WarehouseId = stock.WarehouseId

            }).ToList();
            _response.StatusCode = HttpStatusCode.OK;
            return (_response);
        }

        public ApiResponse AddStockInWarehouseSV(WarehouseDTO addWarehouseDTO, int userId)
        {
            try
            {
                // Create Warehouse Object Here
                var count = 1;
                var stockEntities = addWarehouseDTO.Stocks.Select(stock =>
                {
                    count += 1;
                    return new Stock
                    {
                        // Map properties from DTO to Entity
                        IndexForDeletion = count,
                        ProductId = stock.ProductId,
                        Quantity = stock.Quantity,
                        MarginPercentage = stock.MarginPercentage,
                        IsReturnable = stock.IsReturnable,
                        LastUpdated = DateTime.Now,
                        WarehouseId = userId
                    };
                }).ToList();

                _unitOfWork.Stock.AddRange(stockEntities);
                _response.Result = stockEntities;
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

        public ApiResponse EditStockInWarehouseSV(int indexId, StockDTO editWarehouseDTO, int userId)
        {
            try
            {
                Stock stockFromDb = _unitOfWork.Stock.Get(u => u.WarehouseId == userId && u.IndexForDeletion == indexId);

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

        public ApiResponse RemoveStockSV(int indexId, int userId)
        {
            Stock stockForDeletion = _unitOfWork.Stock.Get(u => u.WarehouseId == userId && u.IndexForDeletion == indexId);
            _unitOfWork.Stock.Remove(stockForDeletion);
            _unitOfWork.Save();
            _response.Result = stockForDeletion;
            _response.StatusCode = HttpStatusCode.OK;
            return (_response);
        }
    }
}
