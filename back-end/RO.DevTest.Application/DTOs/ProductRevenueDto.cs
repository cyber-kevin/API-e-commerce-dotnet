namespace RO.DevTest.Application.DTOs;

public class ProductRevenueDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
}
