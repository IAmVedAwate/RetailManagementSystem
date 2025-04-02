using RetailManagementSystem.DataAccess.Data;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.Retailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.DataAccess.Repository
{
    public class RetailerRepository : Repository<RetailerUser>, IRetailerRepository 
    { 
        private readonly ApplicationDbContext _db;
        public RetailerRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(RetailerUser retailerUser)
        {
            _db.RetailerUsers.Update(retailerUser);
        }
    }
}
