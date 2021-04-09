﻿using Beecow.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beecow.Interfaces
{
    public interface IOrderService
    {
        Task<List<Order>> GetOrdersByCustomerId(int id);
    }
}
