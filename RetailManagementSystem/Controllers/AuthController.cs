using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RetailManagementSystem.DataAccess.Repository;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.Delivery;
using RetailManagementSystem.Models.Models.DTO;
using RetailManagementSystem.Models.Models.Retailer;
using RetailManagementSystem.Models.Models.Store;
using RetailManagementSystem.Services.IServices;
using RetailManagementSystem.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace RetailManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfServices _unitOfServices;
        private ApiResponse _response;

        private string SecretKey;
        public AuthController(IUnitOfServices unitOfServices, IConfiguration configuration)
        {
            _unitOfServices = unitOfServices;
            _response = new ApiResponse();
            SecretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            ApiResponse result = await _unitOfServices.AuthService.LoginSV(model, SecretKey);
            return Ok(result);
        }
        

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequestDTO model)
        {
            ApiResponse result = await _unitOfServices.AuthService.RegisterSV(model, SecretKey);
            return Ok(result);
        }
    }
}
