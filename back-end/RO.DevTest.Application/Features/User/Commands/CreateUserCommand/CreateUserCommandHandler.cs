using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Exception;
using System.Linq;

namespace RO.DevTest.Application.Features.User.Commands.CreateUserCommand;

/// <summary>
/// Command handler for the creation of <see cref="Domain.Entities.User"/>
/// </summary>
public class CreateUserCommandHandler(IIdentityAbstractor identityAbstractor) : IRequestHandler<CreateUserCommand, CreateUserResult> {
    private readonly IIdentityAbstractor _identityAbstractor = identityAbstractor;

    public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken) {
        CreateUserCommandValidator validator = new();
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);

        if(!validationResult.IsValid) {
            var validationErrors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new BadRequestException($"Erro de validação: {validationErrors}");
        }

        Domain.Entities.User newUser = request.AssignTo();
        IdentityResult userCreationResult = await _identityAbstractor.CreateUserAsync(newUser, request.Password);
        if(!userCreationResult.Succeeded) {
            var identityErrors = string.Join("; ", userCreationResult.Errors.Select(e => e.Description));
            throw new BadRequestException($"Falha ao criar usuário: {identityErrors}");
        }

        IdentityResult userRoleResult = await _identityAbstractor.AddToRoleAsync(newUser, request.Role);
        if(!userRoleResult.Succeeded) {
            var roleErrors = string.Join("; ", userRoleResult.Errors.Select(e => e.Description));
            throw new BadRequestException($"Falha ao adicionar papel ao usuário: {roleErrors}");
        }

        return new CreateUserResult(newUser);
    }
}
