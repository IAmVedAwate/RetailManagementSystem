﻿using Microsoft.AspNetCore.Http;
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
using RetailManagementSystem.Services.IServices;

namespace RetailManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        
        private IUnitOfServices _unitOfServices;
        private ApiResponse _response;
        public WarehouseController(IUnitOfServices unitOfServices)
        {
            _unitOfServices = unitOfServices;
            _response = new ApiResponse();
        }

        [HttpGet("all/{subId:int}")]
        [Authorize]
        public async Task<IActionResult> GetAllWarehouses(int subId)
        {
            ApiResponse result = _unitOfServices.WarehouseService.GetAllWarehousesSV(subId);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = SD.Role_Retailer)]
        public async Task<IActionResult> GetWarehouse()
        {
            ApiResponse result = _unitOfServices.WarehouseService.GetWarehouseSV(Convert.ToString(User.FindFirstValue(ClaimTypes.Email)));
            return Ok(result);
        }

        [HttpGet("Msgs")]
        [Authorize(Roles = SD.Role_Retailer)]
        public async Task<IActionResult> GetMessages()
        {
            ApiResponse result = _unitOfServices.WarehouseService.GetMessagesSV(Convert.ToString(User.FindFirstValue(ClaimTypes.Email)));
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles=SD.Role_Retailer)]
        public async Task<IActionResult> AddStockInWarehouse([FromBody] StockDTO addStockDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse result = _unitOfServices.WarehouseService.AddStockInWarehouseSV(addStockDTO, Convert.ToString(User.FindFirstValue(ClaimTypes.Email)));
                    _response = result;
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
            return Ok(_response);
        }
        [HttpPut("{indexId:int}", Name = "EditStockInWarehouse")]
        [Authorize(Roles=SD.Role_Retailer)]
        public async Task<IActionResult> EditStockInWarehouse(int indexId, [FromBody] StockDTO editWarehouseDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse result = _unitOfServices.WarehouseService.EditStockInWarehouseSV(indexId, editWarehouseDTO, Convert.ToString(User.FindFirstValue(ClaimTypes.Email)));
                    _response = result;
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
            return Ok(_response);
        }
        [HttpDelete("{indexId:int}", Name = "RemoveStock")]
        [Authorize(Roles=SD.Role_Retailer)]
        public async Task<IActionResult> RemoveStock(int indexId)
        {
            try
            {
                if (indexId != null || indexId != 0)
                {
                    ApiResponse result = _unitOfServices.WarehouseService.RemoveStockSV(indexId, Convert.ToString(User.FindFirstValue(ClaimTypes.Email)));
                    _response = result;
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpGet("Order/History")]
        //[Authorize(Roles = SD.Role_Retailer)]
        public async Task<IActionResult> CopyGetBillCopyForRetailer()
        {
            ApiResponse result = _unitOfServices.WarehouseService.GetBillCopyForRetailerSV(Convert.ToString(User.FindFirstValue(ClaimTypes.Email)));
            return Ok(result);
        }
    }
}
