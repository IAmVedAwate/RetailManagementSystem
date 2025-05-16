using RetailManagementSystem.DataAccess.Data;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public ISubCategoryRepository SubCategory { get; private set; }
        public IProductRepository Product { get; private set; }
        public IAdvertisementRepository Advertisement { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IStoreRepository StoreUser { get; private set; }
        public IRetailerRepository RetailerUser { get; private set; }
        public IAdminRepository AdminUser { get; private set; }
        public IBillRepository Bill { get; private set; }
        public IDeliveryRepository Delivery { get; private set; }
        public IFeedbackRepository Feedback { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IReturnRepository Return { get; private set; }
        public IStockRepository Stock { get; private set; }
        public IWarehouseRepository Warehouse { get; private set; }
        public IDeliveryUserRepository DeliveryUser { get; private set; }
        public IOrderAssignRepository OrderAssign { get; private set; }
        public IRetailMessagesRepository RetailMessages { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            AdminUser = new AdminRepository(_db);
            Advertisement = new AdvertisementRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            Category = new CategoryRepository(_db);
            SubCategory = new SubCategoryRepository(_db);
            Feedback = new FeedbackRepository(_db);
            Product = new ProductRepository(_db);
            Delivery = new DeliveryRepository(_db);
            DeliveryUser = new DeliveryUserRepository(_db);
            OrderAssign = new OrderAssignRepository(_db);
            Return = new ReturnRepository(_db);
            RetailerUser = new RetailerRepository(_db);
            Stock = new StockRepository(_db);
            Warehouse = new WarehouseRepository(_db);
            Bill  = new BillRepository(_db);
            Order = new OrderRepository(_db);
            StoreUser = new StoreRepository(_db);
            RetailMessages = new RetailMessagesRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
