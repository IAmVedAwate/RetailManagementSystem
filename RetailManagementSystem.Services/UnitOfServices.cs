using RetailManagementSystem.Services.IServices;
using RetailManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetailManagementSystem.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using RetailManagementSystem.DataAccess.Repository;
using RetailManagementSystem.Models.Models;

namespace RetailManagementSystem.Services
{
    public class UnitOfServices : IUnitOfServices
    {
        public ICategoryServices CategoryService {  get; private set; }

        public ISubCategoryServices SubCategoryService { get; private set; }

        public IFeedbackServices FeedbackService { get; private set; }

        public IProductServices ProductService { get; private set; }

        public IWarehouseServices WarehouseService { get; private set; }

        public IAdvertisementServices AdvertisementService { get; private set; }

        public IAuthServices AuthService { get; private set; }

        public IBillServices BillService { get; private set; }
        public IDeliveryServices DeliveryService { get; private set; }

        private readonly IUnitOfWork _unitOfWork;
        private IBlobService _blobService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UnitOfServices(IUnitOfWork unitOfWork, IBlobService blobService, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _blobService = blobService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            CategoryService = new CategoryServices(_unitOfWork);
            SubCategoryService = new SubCategoryServices(_unitOfWork);
            FeedbackService = new FeedbackServices(_unitOfWork);
            ProductService = new ProductServices(_unitOfWork, _blobService);
            WarehouseService = new WarehouseServices(_unitOfWork);
            AdvertisementService = new AdvertisementServices(_unitOfWork);
            AuthService = new AuthServices(_unitOfWork, _userManager, _roleManager);
            DeliveryService = new DeliveryServices(unitOfWork);
            _blobService = blobService;
        }
    }
}
