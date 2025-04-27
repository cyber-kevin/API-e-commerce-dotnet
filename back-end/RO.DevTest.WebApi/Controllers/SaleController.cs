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
using System.Threading.Tasks;
using RO.DevTest.Persistence.Repositories;
using RO.DevTest.Application.DTOs;

namespace RO.DevTest.WebApi.Controllers;

[Route("api/[controller]")]
public class SaleController : ControllerBase
{
    private readonly SaleRepository _saleRepository;
    private readonly ProductRepository _productRepository;

    public SaleController(SaleRepository saleRepository, ProductRepository productRepository)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
    }

    /// <summary>
    /// Get all sales
    /// </summary>
    /// <returns>List of sales</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllSales()
    {
        var sales = await _saleRepository.GetAllAsync();

        if (sales == null || !sales.Any())
        {
            return Ok(sales);
        }

        var salesDto = sales.Select(sale => new SaleResponseDto
        {
            Id = sale.Id,
            CustomerId = sale.CustomerId,
            Items = sale.Items.Select(item => new ItemSaleResponseDto
            {
            Id = item.Id,
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice
            }).ToList(),
            TotalValue = sale.TotalValue,
            Status = sale.Status,
            SaleDate = sale.SaleDate,
            PaymentMethod = sale.PaymentMethod,
            Observations = sale.Observations
        }).ToList();

        return Ok(salesDto);
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

        var saleDto = new SaleResponseDto
        {
            Id = sale.Id,
            CustomerId = sale.CustomerId,
            Items = sale.Items.Select(item => new ItemSaleResponseDto
            {
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice
            }).ToList(),
            TotalValue = sale.TotalValue,
            Status = sale.Status,
            SaleDate = sale.SaleDate,
            PaymentMethod = sale.PaymentMethod,
            Observations = sale.Observations
        };

        return Ok(saleDto);
    }

    /// <summary>
    /// Create a new sale
    /// </summary>
    /// <param name="sale">Sale to create</param>
    /// <returns>Created sale</returns>
    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] Sale saleRequest)
    {
        if (saleRequest == null)
        {
            return BadRequest("Dados inválidos.");
        }

        var sale = new Sale
        {
            CustomerId = saleRequest.CustomerId,
            Items = saleRequest.Items?.Select(item => new ItemSale
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList() ?? new List<ItemSale>(),
            PaymentMethod = saleRequest.PaymentMethod,
            Observations = saleRequest.Observations,
            SaleDate = DateTime.UtcNow,
        };

        await _saleRepository.AddAsync(sale);
        await _saleRepository.SaveChangesAsync();

        foreach (var item in sale.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                product.QuantityStock -= item.Quantity;
                product.ItemSales.Add(item);
                await _productRepository.UpdateAsync(product);
                await _productRepository.SaveChangesAsync();
            }
        }

        var saleResponseDto = new SaleResponseDto
        {
            Id = sale.Id,
            CustomerId = sale.CustomerId,
            Items = sale.Items.Select(item => new ItemSaleResponseDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList(),
            TotalValue = sale.TotalValue,
            Status = sale.Status,
            SaleDate = sale.SaleDate,
            PaymentMethod = sale.PaymentMethod,
            Observations = sale.Observations
        };

        return CreatedAtAction(nameof(GetSaleById), new { id = sale.Id }, saleResponseDto);
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
