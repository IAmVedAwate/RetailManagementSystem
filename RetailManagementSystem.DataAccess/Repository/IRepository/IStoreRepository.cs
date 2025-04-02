using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.DataAccess.Repository.IRepository
{
    public interface IStoreRepository : IRepository<StoreUser>
    {
        void Update(StoreUser storeUser);
    }
}
