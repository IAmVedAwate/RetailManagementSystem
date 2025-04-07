using RetailManagementSystem.Models.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.DataAccess.Repository.IRepository
{
    public interface ISubCategoryRepository : IRepository<SubCategory>
    {
        void Update(SubCategory subCategory);
        int CountAll();
    }
}
