using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Utility;
using RetailManagementSystem.Models.Models;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.Delivery;
using RetailManagementSystem.Models.Models.DTO;
using RetailManagementSystem.Models.Models.Retailer;
using RetailManagementSystem.Models.Models.Store;
using RetailManagementSystem.Services.IServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApiResponse _response;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthServices(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _response = new ApiResponse();
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApiResponse> LoginSV(LoginRequestDTO model, string SecretKey)
        {
            ApplicationUser userFromDb = _unitOfWork.ApplicationUser.GetAll()
                .FirstOrDefault(x => x.UserName.ToLower() == model.Username.ToLower());
            var IsValid = await _userManager.CheckPasswordAsync(userFromDb, model.Password);
            if (!IsValid)
            {
                _response.Result = new LoginResponseDTO();
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = ["Username or Password is Incorrect !!!"];
                return _response;
            }
            var Roles = await _userManager.GetRolesAsync(userFromDb);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(SecretKey);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("fullName",userFromDb.Name),
                    new Claim(ClaimTypes.NameIdentifier,userFromDb.Id.ToString()),
                    new Claim(ClaimTypes.Email,userFromDb.UserName.ToString()),
                    new Claim(ClaimTypes.Role,Roles.FirstOrDefault()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = handler.CreateToken(tokenDescriptor);

            LoginResponseDTO loginResponse = new LoginResponseDTO()
            {
                Email = userFromDb.Email,
                Token = handler.WriteToken(token),
                Role = Roles.FirstOrDefault()
            };
            if (loginResponse.Email == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = ["Error in Loging In !!!"];
                return _response;
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            return _response;
        }

        public async Task<ApiResponse> RegisterSV(RegisterRequestDTO model, string SecretKey)
        {
            ApplicationUser userFromDb = _unitOfWork.ApplicationUser.GetAll()
                .FirstOrDefault(x => x.UserName.ToLower() == model.Username.ToLower());
            try
            {
                if (userFromDb != null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = ["Username Already Exists!!!"];
                    return _response;
                }

                ApplicationUser newUser = new ApplicationUser()
                {
                    UserName = model.Username,
                    Email = model.Username,
                    NormalizedEmail = model.Username.ToUpper(),
                    Name = model.Name,
                    PhoneNumber = model.Phone
                };
                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Store));
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Retailer));
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Delivery));
                    }
                    await _userManager.AddToRoleAsync(newUser, model.Role.ToLower());
                    var applicationUserId = newUser.Id;
                    switch (model.Role.ToLower())
                    {
                        case SD.Role_Retailer:
                            Warehouse warehouse = new Warehouse();
                            _unitOfWork.Warehouse.Add(warehouse);
                            _unitOfWork.Save();
                            RetailerUser newRetailerUser = new RetailerUser
                            {
                                RetailName = model.RetailName,
                                Email = model.Username,
                                OwnerName = model.Username,
                                GSTNumber = model.GSTNumber,
                                ProfilePhoto = model.ProfilePhoto,
                                Phone = model.Phone,
                                Address = model.Address,
                                WarehouseId = warehouse.Id
                            };
                            _unitOfWork.RetailerUser.Add(newRetailerUser);
                            break;

                        case SD.Role_Delivery:
                            DeliveryUser newDeliveryUser = new DeliveryUser
                            {
                                DeliveryUserName = model.DeliveryUserName,
                                Email = model.Username,
                                Adhar = model.Adhar,
                                ProfilePhoto = model.ProfilePhoto,
                                Phone = model.Phone,
                                Address = model.Address  // Delivery-specific field
                            };
                            _unitOfWork.DeliveryUser.Add(newDeliveryUser);
                            break;

                        case SD.Role_Store:
                            StoreUser newStoreUser = new StoreUser
                            {
                                StoreName = model.StoreName,
                                Email = model.Username,
                                GSTNumber = model.GSTNumber,
                                PhotoOfStore = model.PhotoOfStore,
                                ProfilePhoto = model.ProfilePhoto,
                                Phone = model.Phone,
                                Address = model.Address,
                                DateJoined = model.DateJoined  // Store-specific field
                            };
                            _unitOfWork.StoreUser.Add(newStoreUser);

                            break;

                        case SD.Role_Admin:
                            AdminUser newAdminUser = new AdminUser
                            {
                                AdminName = model.AdminName,
                                Email = model.Username,
                                Phone = model.Phone,
                                ProfilePhoto = model.ProfilePhoto
                            };
                            _unitOfWork.AdminUser.Add(newAdminUser);

                            break;

                        default:
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.IsSuccess = false;
                            _response.ErrorMessages = new List<string> { "Invalid role provided" };
                            return _response;
                    }
                    _unitOfWork.Save();
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    return _response;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

                return _response;
            }
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Error While Registering User!!!");

            return _response;
        }
    }
}
