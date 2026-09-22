using System.Net;
using BuildingBlocks.Application.Envelope;
using BuildingBlocks.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace OrderProcessing.Api.Middleware;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, code, message) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                "VALIDATION_ERROR",
                validationEx.Errors.Any()
                    ? string.Join("; ", validationEx.Errors.Select(e => e.ErrorMessage))
                    : validationEx.Message),
            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                "NOT_FOUND",
                notFoundEx.Message),
            DomainException domainEx => (
                HttpStatusCode.UnprocessableEntity,
                "DOMAIN_ERROR",
                domainEx.Message),
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                "UNAUTHORIZED",
                "Access denied."),
            _ => (
                HttpStatusCode.InternalServerError,
                "INTERNAL_ERROR",
                "An unexpected error occurred.")
        };

        logger.LogError(exception, "Unhandled exception: {Code} - {Message}", code, message);

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/json";

        var response = ResponseEnvelope<object>.Failure(code, message);
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
