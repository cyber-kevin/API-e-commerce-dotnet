using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Exception;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RO.DevTest.Application.Features.Auth.Commands.LoginCommand;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse> {
    private readonly IIdentityAbstractor _identityAbstractor;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(
        IIdentityAbstractor identityAbstractor,
        IConfiguration configuration)
    {
        _identityAbstractor = identityAbstractor;
        _configuration = configuration;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken) {

        Console.WriteLine($"LoginCommandHandler: {request.Username} - {request.Password}");

        var user = await _identityAbstractor.FindUserByEmailAsync(request.Username);
        
        if (user == null)
        {
            throw new BadRequestException("Usuário ou senha inválidos.");
        }

        var signInResult = await _identityAbstractor.PasswordSignInAsync(user, request.Password);
        
        if (!signInResult.Succeeded)
        {
            throw new BadRequestException("Usuário ou senha inválidos.");
        }

        var roles = await _identityAbstractor.GetUserRolesAsync(user);

        var token = GenerateJwtToken(user, roles);
        
        return new LoginResponse
        {
            AccessToken = token,
            IssuedAt = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddHours(1),
            Roles = roles
        };
    }

    private string GenerateJwtToken(Domain.Entities.User user, IList<string> roles)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "DefaultSecretKeyForDevelopment12345678901234"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "DefaultIssuer",
            audience: _configuration["Jwt:Audience"] ?? "DefaultAudience",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
