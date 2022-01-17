using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;

namespace TOMI.Services.Interfaces.CustomerService
{
    public interface ICustomerService
    {
        Task<Customer> SaveCustomer(CustomerModel customer);
        Task<Customer> DeleteCustomer(Guid id);
        Task<Customer> GetCustomer(Guid id);
        Task<List<Customer>> GetCustomersAsync();

        Task<Customer> GetUserByCustomereAsync(string customerId);
    }
}
