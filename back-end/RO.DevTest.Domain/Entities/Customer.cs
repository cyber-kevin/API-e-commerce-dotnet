using RO.DevTest.Domain.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace RO.DevTest.Domain.Entities
{
    /// <summary>
    /// Represents a customer in the API
    /// </summary>
    public class Customer : BaseEntity
    {
        /// <summary>
        /// Name of the customer
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email of the customer
        /// </summary>
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// CPF of the customer
        /// </summary>
        [Required]
        [MaxLength(14)]
        public string CPF { get; set; } = string.Empty;

        /// <summary>
        /// Phone of the customer
        /// </summary>
        [MaxLength(11)]
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Address of the customer
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Foreign key for the related User
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property for the related User
        /// </summary>
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public Customer() : base() { }
    }
}
