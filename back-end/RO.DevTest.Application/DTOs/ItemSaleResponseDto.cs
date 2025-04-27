namespace RO.DevTest.Application.DTOs;

public class ItemSaleResponseDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public decimal TotalValue => Quantity * UnitPrice;
}
