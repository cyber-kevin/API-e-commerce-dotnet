using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Application.Features.Auth.Commands.LoginCommand;
using RO.DevTest.Application.Features.Auth.Commands.RegisterCommand;

namespace RO.DevTest.WebApi.Controllers;

[Route("api/auth")]
[OpenApiTags("Auth")]
[ApiController]
public class AuthController : Controller {
    private readonly IMediator _mediator;
    public const string LoginEndpoint = "/api/auth/login";
    public const string RegisterEndpoint = "/api/auth/register";

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Realiza o login de um usuário e retorna um token JWT
    /// </summary>
    /// <param name="request">Dados de login (username e password)</param>
    /// <returns>Token JWT e informações de autenticação</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand request)
    {
        Console.WriteLine($"Login attempt: {request.Username}");
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    /// <summary>
    /// Registra um novo usuário no sistema
    /// </summary>
    /// <param name="request">Dados de registro (Email, Password, FirstName, LastName)</param>
    /// <returns>Informações do usuário registrado</returns>
    [HttpPost("register")] // Endpoint descomentado e atualizado
    public async Task<IActionResult> Register([FromBody] RegisterCommand request)
    {
        Console.WriteLine($"Register attempt: {request.Email}");
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}
