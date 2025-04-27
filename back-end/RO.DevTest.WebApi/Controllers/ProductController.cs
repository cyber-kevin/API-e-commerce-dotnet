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

namespace RO.DevTest.WebApi.Controllers;

[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductRepository _productRepository;

    public ProductController(ProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <summary>
    /// Get a paged list of products with filtering and sorting.
    /// </summary>
    /// <param name="parameters">Pagination, filtering, and sorting parameters.</param>
    /// <returns>Paged list of products.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] PaginationParameters parameters)
    {
        var pagedProducts = await _productRepository.GetPagedAsync(parameters, includes: p => p.ItemSales);

        var productDtos = pagedProducts.Items.Select(product => new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            QuantityStock = product.QuantityStock,
            Code = product.Code,
            Active = product.Active,
            ItemSales = product.ItemSales?.Select(itemSale => new ItemSaleResponseDto
            {
                Id = itemSale.Id,
                ProductId = itemSale.ProductId,
                Quantity = itemSale.Quantity,
                UnitPrice = itemSale.UnitPrice
            }).ToList() ?? new List<ItemSaleResponseDto>()
        }).ToList();

        var metadata = new PaginationMetadata(
            pagedProducts.CurrentPage,
            pagedProducts.TotalPages,
            pagedProducts.PageSize,
            pagedProducts.TotalCount
        );

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));

        return Ok(productDtos);
    }

    /// <summary>
    /// Get a product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="product">Product to create</param>
    /// <returns>Created product</returns>
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        if (product == null)
        {
            return BadRequest("O produto não pode ser nulo.");
        }

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="product">Updated product</param>
    /// <returns>No content</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] Product product)
    {
        if (id != product.Id)
            return BadRequest();

        var updated = await _productRepository.UpdateAsync(product);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var deleted = await _productRepository.DeleteByIdAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
