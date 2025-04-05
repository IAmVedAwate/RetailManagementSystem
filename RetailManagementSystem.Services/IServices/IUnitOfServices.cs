using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Services.IServices
{
    public interface IUnitOfServices
    {
        public ICategoryServices CategoryService { get; }
        public ISubCategoryServices SubCategoryService { get; }
        public IFeedbackServices FeedbackService { get; }
        public IProductServices ProductService { get; }
        public IWarehouseServices WarehouseService { get; }
        public IAdvertisementServices AdvertisementService { get; }
        public IAuthServices AuthService { get; }
        public IBillServices BillService { get; }
        public IDeliveryServices DeliveryService { get; }
    }
}
