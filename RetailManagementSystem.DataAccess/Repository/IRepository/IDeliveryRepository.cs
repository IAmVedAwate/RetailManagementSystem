﻿using RetailManagementSystem.Models.Models.Admin;
using RetailManagementSystem.Models.Models.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.DataAccess.Repository.IRepository
{
    public interface IDeliveryRepository : IRepository<Delivery>
    {
        void Update(Delivery delivery);
    }
}
