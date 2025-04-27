using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Application.DTOs;

public class SaleResponseDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public List<ItemSaleResponseDto> Items { get; set; } = new();
    public decimal TotalValue { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Pending;
    public DateTime SaleDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string Observations { get; set; } = string.Empty;
}
