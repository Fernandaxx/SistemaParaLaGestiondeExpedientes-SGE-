using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Comun;
using SGE.Infraestructura.Comun;

namespace SGE.WebApi.Errores;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            EntidadNoEncontradaException => (StatusCodes.Status404NotFound, "Entidad no encontrada", exception.Message),
            AutorizacionException => (StatusCodes.Status403Forbidden, "Acceso denegado", exception.Message),
            DominioException => (StatusCodes.Status400BadRequest, "Error de dominio", exception.Message),
            RepositorioException => (StatusCodes.Status500InternalServerError, "Error de repositorio", "No se pudo completar la operación sobre la persistencia."),
            _ => (StatusCodes.Status500InternalServerError, "Error interno", "Ocurrió un error inesperado.")
        };

        if (statusCode >= StatusCodes.Status500InternalServerError) {
            _logger.LogError(exception, "Error no controlado al procesar {Path}.", httpContext.Request.Path);
        } else {
            _logger.LogWarning(exception, "Error controlado al procesar {Path}.", httpContext.Request.Path);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";
        await JsonSerializer.SerializeAsync(httpContext.Response.Body, problemDetails, cancellationToken: cancellationToken);

        return true;
    }
}
