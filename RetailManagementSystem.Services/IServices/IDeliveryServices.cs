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
    public interface IDeliveryServices
    {
        public ApiResponse GetDeliveriesSV();
        public ApiResponse CreateDeliverySV(DeliveryDTO deliveryDTO, string email);
        public ApiResponse AssignedDeliveryByIdSV(int id, string email);
        public ApiResponse AssignDeliveryToUserSV(int id, string email);
        public ApiResponse SubmitReturnsSV(ReturnDTO submitReturnsDTO, string email);
        public ApiResponse GetReturnSV(string index);
    }
}
