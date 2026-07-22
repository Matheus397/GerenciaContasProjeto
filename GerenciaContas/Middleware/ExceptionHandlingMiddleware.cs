using System.Net;
using GerenciaContas.Application.Common;
using GerenciaContas.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace GerenciaContas.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            await EscreverProblema(context, HttpStatusCode.BadRequest, "Requisição inválida", ex.Message);
        }
        catch (ConflitoException ex)
        {
            await EscreverProblema(context, HttpStatusCode.Conflict, "Conflito", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado.");
            await EscreverProblema(context, HttpStatusCode.InternalServerError,
                "Erro interno", "Ocorreu um erro inesperado.");
        }
    }

    private static Task EscreverProblema(HttpContext context, HttpStatusCode status, string titulo, string detalhe)
    {
        var problema = new ProblemDetails
        {
            Status = (int)status,
            Title = titulo,
            Detail = detalhe
        };

        context.Response.StatusCode = problema.Status.Value;
        context.Response.ContentType = "application/problem+json";
        return context.Response.WriteAsJsonAsync(problema);
    }
}
