using MediatR;
using RO.DevTest.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace RO.DevTest.Application.Features.User.Commands.CreateUserCommand;

public class CreateUserCommand : IRequest<CreateUserResult> {
    [Required]
    public string UserName { get; set; } = string.Empty;
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    [Compare("Password", ErrorMessage = "As senhas não conferem.")]
    public string PasswordConfirmation { get; set; } = string.Empty;
    
    public UserRoles Role { get; set; }

    [Required]
    [MaxLength(14)]
    public string CPF { get; set; } = string.Empty;
    
    [MaxLength(11)]
    public string Phone { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;

}
