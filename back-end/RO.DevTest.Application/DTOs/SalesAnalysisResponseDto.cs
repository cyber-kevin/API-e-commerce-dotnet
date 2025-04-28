namespace RO.DevTest.Application.DTOs;

public class SalesAnalysisResponseDto
{
    public int TotalSalesCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<ProductRevenueDto> ProductRevenues { get; set; } = new();
}
