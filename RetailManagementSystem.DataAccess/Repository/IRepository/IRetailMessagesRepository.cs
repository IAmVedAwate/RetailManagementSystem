﻿using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.Retailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.DataAccess.Repository.IRepository
{
    public interface IRetailMessagesRepository : IRepository<RetailMessages>
    {
        void Update(RetailMessages retailMessages);
    }
}
