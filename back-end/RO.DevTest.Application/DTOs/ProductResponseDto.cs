namespace RO.DevTest.Application.DTOs

{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int QuantityStock { get; set; }

        public string Code { get; set; } = string.Empty;

        public bool Active { get; set; }

        public List<ItemSaleResponseDto> ItemSales { get; set; } = new();
    }
}
