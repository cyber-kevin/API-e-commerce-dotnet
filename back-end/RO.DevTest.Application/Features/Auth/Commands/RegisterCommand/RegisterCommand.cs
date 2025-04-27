using MediatR;
using System.ComponentModel.DataAnnotations;

namespace RO.DevTest.Application.Features.Auth.Commands.RegisterCommand;

public class RegisterCommand : IRequest<RegisterResponse>
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(14)]
    public string CPF { get; set; } = string.Empty;
    
    [MaxLength(11)]
    public string Phone { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
}
