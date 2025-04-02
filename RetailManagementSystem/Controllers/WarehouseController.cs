using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.Store;
using RetailManagementSystem.Models.Models;
using System.Net;
using RetailManagementSystem.Models.Models.Retailer;
using RetailManagementSystem.Models.Models.DTO;
using System.Security.Claims;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using RetailManagementSystem.Utility;

namespace RetailManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        
        private readonly IUnitOfWork _unitOfWork;
        private ApiResponse _response;
        public WarehouseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new ApiResponse();
        }

        private int InitializeWarehouseId()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email != null)
            {
                var mainWarehouseOwner = _unitOfWork.RetailerUser.Get(u => u.Email == email);
                return mainWarehouseOwner?.WarehouseId ?? 0;
            }
            return 0;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetWarehouse()
        {
            IEnumerable<Stock> stocksFromDb = _unitOfWork.Stock.GetAll(u => u.WarehouseId == InitializeWarehouseId(), includeProperties: ["Product"]);
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
            return Ok(_response);
        }

        [HttpPost]
        [Authorize(Roles=SD.Role_Retailer)]
        public async Task<IActionResult> AddStockInWarehouse([FromBody] WarehouseDTO addWarehouseDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Create Warehouse Object Here
                    var count = 1;
                    var stockEntities = addWarehouseDTO.Stocks.Select(stock =>
                    {
                        count +=1;
                        return new Stock
                        {
                            // Map properties from DTO to Entity
                            IndexForDeletion = count,
                            ProductId = stock.ProductId,
                            Quantity = stock.Quantity,
                            MarginPercentage = stock.MarginPercentage,
                            IsReturnable = stock.IsReturnable,
                            LastUpdated = DateTime.Now,
                            WarehouseId = InitializeWarehouseId()
                        };
                    }).ToList();

                    _unitOfWork.Stock.AddRange(stockEntities);
                    _response.Result = stockEntities;
                    _response.StatusCode = HttpStatusCode.Created;
                }
                else
                {
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
            _unitOfWork.Save();
            return Ok(_response);
        }
        [HttpPut("{indexId:int}", Name = "EditStockInWarehouse")]
        [Authorize(Roles=SD.Role_Retailer)]
        public async Task<IActionResult> EditStockInWarehouse(int indexId, [FromForm] StockDTO editWarehouseDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Stock stockFromDb = _unitOfWork.Stock.Get(u=>u.WarehouseId==InitializeWarehouseId() && u.IndexForDeletion == indexId);

                    stockFromDb.ProductId = editWarehouseDTO.ProductId;
                    stockFromDb.Quantity = editWarehouseDTO.Quantity;
                    stockFromDb.MarginPercentage = editWarehouseDTO.MarginPercentage;
                    stockFromDb.IsReturnable = editWarehouseDTO.IsReturnable;
                    stockFromDb.LastUpdated = DateTime.Now;
                    stockFromDb.WarehouseId = InitializeWarehouseId();
                    
                    _unitOfWork.Stock.Update(stockFromDb);
                    
                    _response.Result = editWarehouseDTO;
                    _response.StatusCode = HttpStatusCode.Created;
                }
                else
                {
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
            _unitOfWork.Save();
            return Ok(_response);
        }
        [HttpDelete("{indexId:int}", Name = "RemoveStock")]
        [Authorize(Roles=SD.Role_Retailer)]
        public async Task<IActionResult> RemoveStock(int indexId)
        {
            Stock stockForDeletion = _unitOfWork.Stock.Get(u => u.WarehouseId == InitializeWarehouseId() && u.IndexForDeletion == indexId);
            _unitOfWork.Stock.Remove(stockForDeletion);
            _unitOfWork.Save();
            _response.Result = stockForDeletion;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
    }
}
