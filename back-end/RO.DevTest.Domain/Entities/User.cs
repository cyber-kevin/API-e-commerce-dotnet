using Microsoft.AspNetCore.Identity;
using RO.DevTest.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace RO.DevTest.Domain.Entities;

/// <summary>
/// Represents a <see cref="IdentityUser"/> in the API
/// </summary>
public class User : IdentityUser {
    /// <summary>
    /// Name of the user
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property for the related Customer
    /// </summary>
    public virtual Customer? Customer { get; set; }

    public User() : base() { }
}
