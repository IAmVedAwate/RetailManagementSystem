using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public ICategoryRepository Category { get; }
        public ISubCategoryRepository SubCategory { get; }
        public IProductRepository Product { get; }
        public IAdvertisementRepository Advertisement { get; }
        public IApplicationUserRepository ApplicationUser { get; }
        public IStoreRepository StoreUser { get; }
        public IRetailerRepository RetailerUser { get; }
        public IAdminRepository AdminUser { get; }
        public IBillRepository Bill { get; }
        public IDeliveryRepository Delivery { get; }
        public IFeedbackRepository Feedback { get; }
        public IOrderRepository Order { get; }
        public IReturnRepository Return { get; }
        public IStockRepository Stock { get; }
        public IWarehouseRepository Warehouse { get; }
        public IDeliveryUserRepository DeliveryUser { get; }
        public IOrderAssignRepository OrderAssign { get; }
        void Save();
    }
}
