using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Domain.Exception;
using System.Net;

namespace RO.DevTest.WebApi.Middleware;

public static class ExceptionHandlerMiddleware
{
    public static void UseCustomExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    context.Response.ContentType = "application/problem+json";
                    var exception = contextFeature.Error;

                    HttpStatusCode statusCode;
                    string title;
                    string detail;

                    switch (exception)
                    {
                        case BadRequestException badRequestException:
                            statusCode = HttpStatusCode.BadRequest;
                            title = "Bad Request";
                            detail = badRequestException.Message;
                            break;
                        default:
                            statusCode = HttpStatusCode.InternalServerError;
                            title = "An unexpected error occurred.";
                            detail = env.IsDevelopment() ? exception.ToString() : "Internal Server Error. Please try again later.";
                            break;
                    }

                    context.Response.StatusCode = (int)statusCode;

                    var problemDetails = new ProblemDetails
                    {
                        Status = (int)statusCode,
                        Title = title,
                        Detail = detail,
                        Instance = context.Request.Path
                    };

                    await context.Response.WriteAsJsonAsync(problemDetails);
                }
            });
        });
    }
}
