using RO.DevTest.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RO.DevTest.Domain.Entities;

/// <summary>
/// Represents an individual item within a sale
/// /// </summary>
public class ItemSale : BaseEntity
{
    /// <summary>
    /// Sale ID of the item sale
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Sale of the item sale
    [JsonIgnore]
    public virtual Sale Sale { get; set; } = null!;

    /// <summary>
    /// Product ID of the item sale
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    [JsonIgnore]
    /// </summary>
    public virtual Product Product { get; set; } = null!;

    /// <summary>
    /// Quantity of the product sold in the item sale
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Unit price of the product in the item sale
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Total value of the item sale (Quantity * UnitPrice)
    /// </summary>
    public decimal TotalValue => Quantity * UnitPrice;

    public ItemSale() : base() { }
}
