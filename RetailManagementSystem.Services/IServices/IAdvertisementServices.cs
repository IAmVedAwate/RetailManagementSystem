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
    public interface IAdvertisementServices
    {
        public ApiResponse GetAdvertisementsSV();
        public ApiResponse CreateAdvertisementSV(AdvertisementDTO createAdvertisementDTO, string email);
        public ApiResponse UpdateAdvertisementSV(int id, AdvertisementDTO updateAdvertisementDTO);
        public ApiResponse DeleteAdvertisementSV(int id);
    }
}
