using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;
using TOMI.Services.Interfaces.CustomerService;
using TOMI.Services.Models;

namespace TOMI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerController> _logger;
        public CustomerController(ILogger<CustomerController> logger, ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into CustomerController");
        }
        #region Public methods

        /// <summary>
        /// AddCustomer
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AddCustomer")]
        public async Task<IActionResult> AddCustomerAsync(CustomerModel customer)
        {
            try
            {
                var CustomerRequest = _mapper.Map<Customer>(customer);
                var result = await _customerService.SaveCustomer(customer);


                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// DeleteCustomer
        /// </summary>
        /// <returns></returns>
        [HttpDelete("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomerAsync(Guid customerId)
        {
            try
            {
                var result = await _customerService.DeleteCustomer(customerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// GetCustomerList
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCustomerList")]
        public async Task<IActionResult> GetCustomerListAsync()
        {
            try
            {
                var response = await _customerService.GetCustomersAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// GetCustomer
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCustomer/{id}")]
        public async Task<IActionResult> GetCustomerAsync(Guid id)
        {
            try
            {
                var CustomerRequest = _mapper.Map<Customer>(id);
                var response = await _customerService.GetCustomer(CustomerRequest.Id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpGet("GetUsersById/{customerId}")]
        public async Task<IActionResult> GetUsersByIdAsync(string customerId)
        {
            try
            {
                return Ok(await _customerService.GetUserByCustomereAsync(customerId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        #endregion
    }
}
    
