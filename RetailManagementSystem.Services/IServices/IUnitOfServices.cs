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
    }
}
