namespace RO.DevTest.Domain.Common.Models;

/// <summary>
/// Represents the overall result of a sales analysis for a given period.
/// This model resides in the Domain layer.
/// </summary>
public class SalesAnalysisResult
{
    public int TotalSalesCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<ProductRevenueResult> ProductRevenues { get; set; } = new();
}

