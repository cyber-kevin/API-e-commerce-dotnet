using RO.DevTest.Domain.Abstract;
using System;
using System.Collections.Generic;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Domain.Entities;

/// <summary>
/// Represents a sale in the API 
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Customer ID of the sale
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Customer of the sale
    /// </summary>
    public virtual Customer Customer { get; set; } = null!;

    /// <summary>
    /// List of items in the sale
    /// </summary>
    public virtual List<ItemSale> Items { get; set; } = new();

    /// <summary>
    /// Total value of the sale (sum of item totals)
    /// </summary>
    public decimal TotalValue => Items.Sum(item => item.TotalValue);

    /// <summary>
    /// Status of the sale
    /// </summary>
    public SaleStatus Status { get; set; } = SaleStatus.Pending;

    /// <summary>
    /// Date of the sale
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Payment method of the sale
    /// </summary>
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>
    /// Observations of the sale
    /// </summary>
    public string Observations { get; set; } = string.Empty;
}
