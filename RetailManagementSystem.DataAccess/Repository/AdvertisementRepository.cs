using RetailManagementSystem.DataAccess.Data;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using RetailManagementSystem.Models.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.DataAccess.Repository
{
    public class AdvertisementRepository : Repository<Advertisement>, IAdvertisementRepository
    {
        private readonly ApplicationDbContext _db;
        public AdvertisementRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Advertisement advertisement)
        {
            _db.Advertisements.Update(advertisement);
        }
    }
}
