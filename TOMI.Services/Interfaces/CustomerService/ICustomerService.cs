﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;

namespace TOMI.Services.Interfaces.CustomerService
{
    public interface ICustomerService
    {
        Task<Customer> SaveCustomer(Customer customer);
        Task<Customer> DeleteCustomer(Guid id);
        Task<Customer> GetCustomer(Guid id);
        Task<List<Customer>> GetCustomersAsync();
    }
}
