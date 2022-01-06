using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TOMI.Data.Database;
using TOMI.Data.Database.Entities;
using TOMI.Services.Interfaces.CustomerService;
using TOMI.Services.Models;

namespace TOMI.Services.Repository
{
    public class CustomerRepository : ICustomerService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerRepository> _logger;
        private readonly TOMIDataContext _context;
        public CustomerRepository(ILogger<CustomerRepository> logger, TOMIDataContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }
        public async Task<Customer> DeleteCustomer(Guid id)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Customer> GetCustomer(Guid id)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<Customer>> GetCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }
        public async Task<Customer> SaveCustomer(CustomerModel customer)
        {
            Customer exsitingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Name == customer.Name);

            var customers = _mapper.Map<Customer>(customer);
            if (exsitingCustomer == null)
            {
                Customer result = _context.Customers.Add(customers).Entity;
                _context.SaveChanges();
                return result;
            }
            throw new ValidationException("Customer not found!");
        }
    }
}
