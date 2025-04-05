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
    public class SubCategoryRepository : Repository<SubCategory>, ISubCategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public SubCategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(SubCategory subCategory)
        {
            _db.SubCategories.Update(subCategory);
        }
    }
}
