using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Application.DTOs;

public class SaleRequestDto
{
    public Guid CustomerId { get; set; }
    public List<ItemSale> Items { get; set; } = new();
    public PaymentMethod PaymentMethod { get; set; }
    public string Observations { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
}
