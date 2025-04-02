using RetailManagementSystem.DataAccess.Data;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.DataAccess.Repository
{
    public class ReturnRepository : Repository<Return>, IReturnRepository
    {
        private readonly ApplicationDbContext _db;
        public ReturnRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Return returnObj)
        {
            _db.Returns.Update(returnObj);
        }
    }
}
