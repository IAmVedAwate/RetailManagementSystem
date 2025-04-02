using RetailManagementSystem.Services.IServices;
using RetailManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetailManagementSystem.DataAccess.Repository.IRepository;

namespace RetailManagementSystem.Services
{
    public class UnitOfServices : IUnitOfServices
    {
        public ICategoryServices CategoryService {  get; private set; }
        private readonly IUnitOfWork _unitOfWork;
        public UnitOfServices(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
            CategoryService = new CategoryServices(_unitOfWork);
        }
    }
}
