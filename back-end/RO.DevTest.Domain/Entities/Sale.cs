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
    /// Product ID of the sale
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Product of the sale
    /// </summary>
    public virtual Product Product { get; set; } = null!;

    /// <summary>
    /// Quantity of the product sold in the sale
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Total value of the sale
    /// </summary>
    public decimal TotalValue { get; set; }

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
