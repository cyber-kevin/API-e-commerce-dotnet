using RO.DevTest.Domain.Abstract;
using System.ComponentModel.DataAnnotations;

namespace RO.DevTest.Domain.Entities

{
    /// <summary>
    /// Represents a product in the API
    /// </summary>
    public class Product : BaseEntity
    {
        /// <summary>
        /// Name of the product
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the product
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Price of the product
        /// </summary>
        public decimal Price { get; set; } = 0.0m;

        /// <summary>
        /// Amount of the product in stock
        /// </summary>
        public int QuantityStock { get; set; }

        /// <summary>
        /// Code of the product
        /// </summary>
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Whether the product is active
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        /// Sale items associated with this product
        /// </summary>
        public virtual ICollection<ItemSale> ItemSales { get; set; } = new List<ItemSale>();

        public Product() : base() { }
    }
}