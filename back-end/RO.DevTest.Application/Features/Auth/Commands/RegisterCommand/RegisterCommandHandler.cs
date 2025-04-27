using MediatR;
using Microsoft.AspNetCore.Identity;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Enums;
using RO.DevTest.Domain.Exception;

namespace RO.DevTest.Application.Features.Auth.Commands.RegisterCommand;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly IIdentityAbstractor _identityAbstractor;

    public RegisterCommandHandler(IIdentityAbstractor identityAbstractor)
    {
        _identityAbstractor = identityAbstractor;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new BadRequestException("Email e senha são obrigatórios.");
        }

        var existingUser = await _identityAbstractor.FindUserByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new BadRequestException("Este email já está em uso.");
        }

        var user = new Domain.Entities.User
        {
            UserName = request.Email,
            Email = request.Email,
            Name = $"{request.FirstName} {request.LastName}".Trim(),
            EmailConfirmed = true
        };

        var result = await _identityAbstractor.CreateUserAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BadRequestException($"Falha ao criar usuário: {errors}");
        }

        await _identityAbstractor.AddToRoleAsync(user, UserRoles.Customer);

        return new RegisterResponse
        {
            UserId = user.Id,
            Username = user.UserName,
            Email = user.Email,
            Roles = new List<string> { UserRoles.Customer.ToString() }
        };
    }
}
