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
    public class RetailMessagesRepository : Repository<RetailMessages>, IRetailMessagesRepository
    {
        private readonly ApplicationDbContext _db;
        public RetailMessagesRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(RetailMessages retailMessages)
        {
            _db.RetailMessages.Update(retailMessages);
        }
    }
}
