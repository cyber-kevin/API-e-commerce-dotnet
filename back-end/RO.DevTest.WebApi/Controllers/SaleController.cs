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
using RO.DevTest.Application.DTOs;
using RO.DevTest.Domain.Common.Parameters;
using RO.DevTest.Domain.Common.Models;
using RO.DevTest.Domain.Enums;
using System.Linq.Expressions;

namespace RO.DevTest.WebApi.Controllers;

[Route("api/[controller]")]
public class SaleController : ControllerBase
{
    private readonly ISaleRepository _saleRepository;
    private readonly ProductRepository _productRepository;

    public SaleController(ISaleRepository saleRepository, ProductRepository productRepository)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
    }

    /// <summary>
    /// Get sales analysis for a given period.
    /// </summary>
    /// <param name="startDate">Start date of the period.</param>
    /// <param name="endDate">End date of the period.</param>
    /// <returns>Sales analysis data including total sales count, total revenue, and revenue per product.</returns>
    [HttpGet("analysis")]
    [ProducesResponseType(typeof(SalesAnalysisResponseDto), 200)] // API returns DTO
    public async Task<IActionResult> GetSalesAnalysis([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
        {
            return BadRequest("Start date cannot be after end date.");
        }

        var analysisResult = await _saleRepository.GetSalesAnalysisAsync(startDate, endDate);

        var analysisDto = new SalesAnalysisResponseDto
        {
            TotalSalesCount = analysisResult.TotalSalesCount,
            TotalRevenue = analysisResult.TotalRevenue,
            ProductRevenues = analysisResult.ProductRevenues.Select(pr => new ProductRevenueDto
            {
                ProductId = pr.ProductId,
                ProductName = pr.ProductName,
                TotalRevenue = pr.TotalRevenue
            }).ToList()
        };

        return Ok(analysisDto);
    }

    /// <summary>
    /// Get a paged list of sales with filtering and sorting.
    /// </summary>
    /// <param name="parameters">Pagination, filtering, and sorting parameters.</param>
    /// <returns>Paged list of sales.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllSales([FromQuery] PaginationParameters parameters)
    {
        var pagedSales = await _saleRepository.GetPagedAsync(parameters, includes: new Expression<Func<Sale, object>>[] { s => s.Items });

        if (pagedSales.Items == null || !pagedSales.Items.Any())
        {
            var emptyMetadata = new PaginationMetadata(
                pagedSales.CurrentPage,
                pagedSales.TotalPages,
                pagedSales.PageSize,
                pagedSales.TotalCount
            );
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(emptyMetadata));
            return Ok(new List<SaleResponseDto>());
        }

        var salesDto = pagedSales.Items.Select(sale => new SaleResponseDto
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

        var metadata = new PaginationMetadata(
            pagedSales.CurrentPage,
            pagedSales.TotalPages,
            pagedSales.PageSize,
            pagedSales.TotalCount
        );

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));

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
        var sale = await _saleRepository.GetByIdAsync(id, includes: new Expression<Func<Sale, object>>[] { s => s.Items });

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
        };

        return Ok(saleDto);
    }

    /// <summary>
    /// Create a new sale
    /// </summary>
    /// <param name="saleRequestDto">Sale data to create</param>
    /// <returns>Created sale</returns>
    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] SaleRequestDto saleRequestDto)
    {
        if (saleRequestDto == null || saleRequestDto.Items == null || !saleRequestDto.Items.Any())
        {
            return BadRequest("Dados inválidos para a venda ou itens da venda.");
        }

        foreach (var itemDto in saleRequestDto.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
            if (product == null)
            {
                return BadRequest($"Produto com ID {itemDto.ProductId} não encontrado.");
            }
            if (product.QuantityStock < itemDto.Quantity)
            {
                return BadRequest($"Estoque insuficiente para o produto \'{product.Name}\' (ID: {itemDto.ProductId}). Quantidade solicitada: {itemDto.Quantity}, Estoque: {product.QuantityStock}.");
            }
            if (itemDto.Quantity <= 0)
            {
                 return BadRequest($"Quantidade inválida para o produto \'{product.Name}\' (ID: {itemDto.ProductId}). Quantidade deve ser maior que zero.");
            }
        }

        var sale = new Sale
        {
            CustomerId = saleRequestDto.CustomerId,
            PaymentMethod = saleRequestDto.PaymentMethod,
            Observations = saleRequestDto.Observations,
            SaleDate = DateTime.UtcNow,
            Status = SaleStatus.Pending
        };

        sale.Items = new List<ItemSale>();
        foreach (var itemDto in saleRequestDto.Items)
        {
             var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
             sale.Items.Add(new ItemSale
             {
                 ProductId = itemDto.ProductId,
                 Quantity = itemDto.Quantity,
                 UnitPrice = product.Price
             });
        }

        var created = await _saleRepository.AddAsync(sale);
        if (!created)
        {
            return StatusCode(500, "Ocorreu um erro ao criar a venda.");
        }

        foreach (var item in sale.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                product.QuantityStock -= item.Quantity;
                await _productRepository.UpdateAsync(product);
            }
        }
        await _productRepository.SaveChangesAsync(); 

        var saleResponseDto = new SaleResponseDto
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
        };

        return CreatedAtAction(nameof(GetSaleById), new { id = sale.Id }, saleResponseDto);
    }


    /// <summary>
    /// Update an existing sale (Consider what aspects of a sale should be updatable)
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <param name="saleDto">Updated sale data</param>
    /// <returns>No content</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSale(Guid id, [FromBody] SaleRequestDto saleDto) // Use DTO for updates
    {
        var existingSale = await _saleRepository.GetByIdAsync(id, includes: new Expression<Func<Sale, object>>[] { s => s.Items });
        if (existingSale == null)
        {
            return NotFound();
        }

        existingSale.Observations = saleDto.Observations ?? existingSale.Observations;

        var updated = await _saleRepository.UpdateAsync(existingSale);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Delete a sale (Consider implications like stock adjustment)
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSale(Guid id)
    { 
        var saleToDelete = await _saleRepository.GetByIdAsync(id, includes: new Expression<Func<Sale, object>>[] { s => s.Items });
        if (saleToDelete == null)
        {
            return NotFound();
        }

        foreach (var item in saleToDelete.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                product.QuantityStock += item.Quantity;
                await _productRepository.UpdateAsync(product);
            }
        }
        await _productRepository.SaveChangesAsync();

        var deleted = await _saleRepository.DeleteByIdAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
