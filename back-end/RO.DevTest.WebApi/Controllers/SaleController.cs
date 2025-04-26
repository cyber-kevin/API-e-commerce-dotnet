using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Persistence;
using System;
using System.Linq;
using System.Threading.Tasks;
using RO.DevTest.Persistence.Repositories;

namespace RO.DevTest.WebApi.Controllers;

[Route("api/[controller]")]
public class SaleController : ControllerBase
{
    private readonly SaleRepository _saleRepository;

    public SaleController(SaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    /// <summary>
    /// Get all sales
    /// </summary>
    /// <returns>List of sales</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllSales()
    {
        var sales = await _saleRepository.GetAllAsync();
        return Ok(sales);
    }

    /// <summary>
    /// Get a sale by ID
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <returns>Sale</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSaleById(Guid id)
    {
        var sale = await _saleRepository.GetByIdAsync(id);
        if (sale == null)
        {
            return NotFound();
        }
        return Ok(sale);
    }

    /// <summary>
    /// Create a new sale
    /// </summary>
    /// <param name="sale">Sale to create</param>
    /// <returns>Created sale</returns>
    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] Sale sale)
    {
        if (sale == null)
        {
            return BadRequest("A venda não pode ser nula.");
        }

        await _saleRepository.AddAsync(sale);
        await _saleRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSaleById), new { id = sale.Id }, sale);
    }

    /// <summary>
    /// Update an existing sale
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <param name="sale">Updated sale</param>
    /// <returns>No content</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSale(Guid id, [FromBody] Sale sale)
    {
        if (id != sale.Id)
            return BadRequest();

        var updated = await _saleRepository.UpdateAsync(sale);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Delete a sale
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSale(Guid id)
    {
        var deleted = await _saleRepository.DeleteByIdAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
