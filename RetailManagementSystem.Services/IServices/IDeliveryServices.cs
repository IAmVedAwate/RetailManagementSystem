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
        public ApiResponse CreateDeliverySV(DeliveryDTO deliveryDTO, int userId);
        public ApiResponse SubmitReturnsSV(ReturnDTO submitReturnsDTO, int userId);
        public ApiResponse GetReturnSV(string index);
    }
}
