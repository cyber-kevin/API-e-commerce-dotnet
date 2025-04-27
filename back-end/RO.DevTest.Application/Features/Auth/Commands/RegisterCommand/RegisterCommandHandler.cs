using MediatR;
using Microsoft.AspNetCore.Identity;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Enums;
using RO.DevTest.Domain.Exception;
using RO.DevTest.Persistence.Repositories;

namespace RO.DevTest.Application.Features.Auth.Commands.RegisterCommand;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly IIdentityAbstractor _identityAbstractor;
    private readonly CustomerRepository _customerRepository;

    public RegisterCommandHandler(IIdentityAbstractor identityAbstractor, CustomerRepository customerRepository)
    {
        _identityAbstractor = identityAbstractor;
        _customerRepository = customerRepository;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.CPF))
        {
            throw new BadRequestException("Email, senha e CPF são obrigatórios.");
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

        var customer = new Customer
        {
            Name = user.Name,
            Email = user.Email,
            CPF = request.CPF,
            Phone = request.Phone,
            Address = request.Address,
            UserId = user.Id
        };

        await _customerRepository.AddAsync(customer);
        await _customerRepository.SaveChangesAsync();

        return new RegisterResponse
        {
            UserId = user.Id,
            Username = user.UserName,
            Email = user.Email,
            Roles = new List<string> { UserRoles.Customer.ToString() }
        };
    }
}
