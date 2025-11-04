using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Middleware;

/// <summary>
/// Middleware global para tratamento centralizado de exceções.
/// Captura todas as exceções não tratadas e as converte em respostas HTTP padronizadas
/// seguindo o padrão Problem Details (RFC 7807).
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Determinar status code, título e detalhes baseado no tipo de exceção
        var (statusCode, errorCode, title, detail) = GetExceptionDetails(exception);

        // Logar a exceção com nível apropriado
        LogException(context, exception, statusCode);

        // Criar Problem Details
        var problemDetails = CreateProblemDetails(
            context, 
            statusCode, 
            errorCode, 
            title, 
            detail, 
            exception
        );

        // Configurar resposta HTTP
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        // Serializar e enviar
        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }

    private (int StatusCode, string ErrorCode, string Title, string Detail) GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            NotFoundException notFound => (
                StatusCodes.Status404NotFound,
                "NOT_FOUND",
                "Recurso não encontrado",
                notFound.Message
            ),

            ValidationException validation => (
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Erro de validação",
                validation.Message
            ),

            DuplicateException duplicate => (
                StatusCodes.Status409Conflict,
                "DUPLICATE",
                "Recurso duplicado",
                duplicate.Message
            ),

            BusinessException business => (
                business.StatusCode,
                business.ErrorCode,
                "Erro de negócio",
                business.Message
            ),

            // Exceções de infraestrutura/sistema
            _ => (
                StatusCodes.Status500InternalServerError,
                "INTERNAL_ERROR",
                "Erro interno do servidor",
                _env.IsDevelopment() 
                    ? exception.Message 
                    : "Ocorreu um erro inesperado. Nossa equipe foi notificada."
            )
        };
    }

    private void LogException(HttpContext context, Exception exception, int statusCode)
    {
        var logLevel = statusCode >= 500 ? LogLevel.Error : LogLevel.Warning;

        _logger.Log(
            logLevel,
            exception,
            "Erro ao processar requisição {Method} {Path}. " +
            "StatusCode: {StatusCode}, TraceId: {TraceId}, User: {User}",
            context.Request.Method,
            context.Request.Path,
            statusCode,
            context.TraceIdentifier,
            context.User?.Identity?.Name ?? "anonymous"
        );
    }

    private ProblemDetails CreateProblemDetails(
        HttpContext context,
        int statusCode,
        string errorCode,
        string title,
        string detail,
        Exception exception)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        // Adicionar TraceId para rastreabilidade
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;
        problemDetails.Extensions["errorCode"] = errorCode;

        // Se for ValidationException, incluir detalhes dos erros
        if (exception is ValidationException validationEx && validationEx.Errors.Any())
        {
            problemDetails.Extensions["errors"] = validationEx.Errors;
        }

        // Em desenvolvimento, adicionar informações de debug
        if (_env.IsDevelopment() && statusCode >= 500)
        {
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
            
            if (exception.InnerException != null)
            {
                problemDetails.Extensions["innerException"] = new
                {
                    message = exception.InnerException.Message,
                    type = exception.InnerException.GetType().Name
                };
            }
        }

        return problemDetails;
    }
}

/// <summary>
/// Extension method para facilitar o registro do middleware.
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
