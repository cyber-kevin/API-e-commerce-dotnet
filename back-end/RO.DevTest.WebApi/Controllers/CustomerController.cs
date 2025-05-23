using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using RO.DevTest.Domain.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Persistence;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using RO.DevTest.Persistence.Repositories;
using RO.DevTest.Domain.Common.Parameters;
using RO.DevTest.Domain.Common.Models;

namespace RO.DevTest.WebApi.Controllers;

[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly CustomerRepository _customerRepository;

    public CustomerController(CustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    /// <summary>
    /// Get a paged list of customers with filtering and sorting.
    /// </summary>
    /// <param name="parameters">Pagination, filtering, and sorting parameters.</param>
    /// <returns>Paged list of customers.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllCustomers([FromQuery] PaginationParameters parameters)
    {
        var pagedCustomers = await _customerRepository.GetPagedAsync(parameters);

        var metadata = new PaginationMetadata(
            pagedCustomers.CurrentPage,
            pagedCustomers.TotalPages,
            pagedCustomers.PageSize,
            pagedCustomers.TotalCount
        );

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));

        return Ok(pagedCustomers.Items);
    }

    /// <summary>
    /// Get a customer by ID
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>Customer</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomerById(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }
        return Ok(customer);
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    /// <param name="customer">Customer to create</param>
    /// <returns>Created customer</returns>
    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
    {
        if (customer == null)
            return BadRequest("O cliente não pode ser nulo.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _customerRepository.AddAsync(customer); 
        await _customerRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
    }

    /// <summary>
    /// Update an existing customer
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="customer">Updated customer</param>
    /// <returns>No content</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] Customer customer)
    {
        if (id != customer.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _customerRepository.UpdateAsync(customer); 

        if (!updated)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Delete a customer
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(Guid id)
    {
        var deleted = await _customerRepository.DeleteByIdAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
