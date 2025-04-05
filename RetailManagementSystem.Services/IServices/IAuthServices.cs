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
    public interface IAuthServices
    {
        public Task<ApiResponse> LoginSV(LoginRequestDTO model, string SecretKey);
        public Task<ApiResponse> RegisterSV(RegisterRequestDTO model, string SecretKey);
    }
}
