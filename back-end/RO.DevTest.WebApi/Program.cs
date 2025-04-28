using RO.DevTest.Application;
using RO.DevTest.Infrastructure.IoC;
using RO.DevTest.Persistence.IoC;
using RO.DevTest.Persistence.Repositories;
using RO.DevTest.Persistence;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Contracts.Persistance.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Domain.Exception;
using Microsoft.Extensions.Logging;

namespace RO.DevTest.WebApi;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddIdentity<User, IdentityRole>(options => {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<DefaultContext>()
        .AddDefaultTokenProviders();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "R0DevT3stS3cr3tK3yF0rJWTAuthenT1cation!12345"))
            };
        });

        builder.Services.AddProblemDetails();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.InjectPersistenceDependencies()
            .InjectInfrastructureDependencies();

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(ApplicationLayer).Assembly,
                typeof(Program).Assembly
            );
        });

        builder.Services.AddScoped<ProductRepository>();
        builder.Services.AddScoped<CustomerRepository>();
        builder.Services.AddScoped<ISaleRepository, SaleRepository>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<DefaultContext>();
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Ocorreu um erro ao aplicar as migrações do banco de dados.");
            }
        }

        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = exceptionHandlerFeature?.Error;

                var statusCode = HttpStatusCode.InternalServerError;
                var title = "Erro Interno do Servidor";
                var detail = "Ocorreu um erro inesperado. Tente novamente mais tarde.";

                if (exception != null)
                {
                    if (app.Environment.IsDevelopment())
                    {
                         var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                         logger.LogError(exception, "Unhandled exception.");
                         detail = exception.ToString();
                    }

                    if (exception is Npgsql.PostgresException pgEx && pgEx.SqlState == "42P01")
                    {
                         statusCode = HttpStatusCode.InternalServerError;
                         title = "Erro de Banco de Dados";
                         detail = "A estrutura do banco de dados parece estar incompleta ou desatualizada. Verifique se as migrações foram aplicadas corretamente.";
                    }
                    else if (exception is BadRequestException badRequestException)
                    {
                        statusCode = HttpStatusCode.BadRequest;
                        title = "Requisição Inválida";
                        detail = badRequestException.Message;
                    }
                }

                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = "application/problem+json";

                var problemDetails = new ProblemDetails
                {
                    Status = (int)statusCode,
                    Title = title,
                    Detail = detail,
                    Instance = context.Request.Path
                };

                await context.Response.WriteAsJsonAsync(problemDetails);
            });
        });

        if(app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        } else {
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
