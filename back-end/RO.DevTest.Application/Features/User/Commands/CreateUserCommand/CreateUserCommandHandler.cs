using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Exception;
using RO.DevTest.Persistence.Repositories;
using System.Linq;

namespace RO.DevTest.Application.Features.User.Commands.CreateUserCommand;

/// <summary>
/// Command handler for the creation of <see cref="Domain.Entities.User"/> and associated <see cref="Domain.Entities.Customer"/>
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResult>
{
    private readonly IIdentityAbstractor _identityAbstractor;
    private readonly CustomerRepository _customerRepository; 

    public CreateUserCommandHandler(IIdentityAbstractor identityAbstractor, CustomerRepository customerRepository)
    {
        _identityAbstractor = identityAbstractor;
        _customerRepository = customerRepository;
    }

    public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        CreateUserCommandValidator validator = new();
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var validationErrors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new BadRequestException($"Erro de validação: {validationErrors}");
        }

        var existingUser = await _identityAbstractor.FindUserByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new BadRequestException("Este email já está em uso.");
        }

        var newUser = new Domain.Entities.User
        {
            UserName = request.UserName,
            Email = request.Email,
            Name = request.Name,
            EmailConfirmed = true
        };

        IdentityResult userCreationResult = await _identityAbstractor.CreateUserAsync(newUser, request.Password);
        if (!userCreationResult.Succeeded)
        {
            var identityErrors = string.Join("; ", userCreationResult.Errors.Select(e => e.Description));
            throw new BadRequestException($"Falha ao criar usuário: {identityErrors}");
        }

        IdentityResult userRoleResult = await _identityAbstractor.AddToRoleAsync(newUser, request.Role);
        if (!userRoleResult.Succeeded)
        {
            var roleErrors = string.Join("; ", userRoleResult.Errors.Select(e => e.Description));
            throw new BadRequestException($"Falha ao adicionar papel ao usuário: {roleErrors}");
        }

        var customer = new Customer
        {
            Name = newUser.Name,
            Email = newUser.Email,
            CPF = request.CPF,
            Phone = request.Phone,
            Address = request.Address,
            UserId = newUser.Id
        };

        try
        {
            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync(); 
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Falha ao criar o cliente associado ao usuário.", ex);
        }

        return new CreateUserResult(newUser);
    }
}
