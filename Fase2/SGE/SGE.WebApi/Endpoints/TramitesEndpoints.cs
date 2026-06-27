using System.Security.Claims;
using SGE.Aplicacion.Tramites;
using SGE.Dominio.Tramites;
using SGE.WebApi.Comun;

namespace SGE.WebApi.Endpoints;

public static class TramitesEndpoints
{
    public static IEndpointRouteBuilder MapTramitesEndpoints(this IEndpointRouteBuilder app)
    {
        var expedienteGroup = app.MapGroup("/expedientes/{expedienteId:guid}/tramites")
            .WithTags("Trámites")
            .RequireAuthorization();

        expedienteGroup.MapGet("", (Guid expedienteId, ListarTramitesPorExpedienteUseCase useCase) =>
        {
            var response = useCase.Ejecutar(new ListarTramitesPorExpedienteRequest(expedienteId));
            return Results.Ok(response);
        })
        .Produces<ListarTramitesPorExpedienteResponse>();

        expedienteGroup.MapPost("", (Guid expedienteId, AgregarTramiteApiRequest request, ClaimsPrincipal user, AgregarTramiteUseCase useCase) =>
        {
            var response = useCase.Ejecutar(new AgregarTramiteRequest(
                expedienteId,
                request.Etiqueta,
                request.Contenido,
                user.ObtenerUserId()));

            return Results.Created($"/tramites/{response.Id}", response);
        })
        .Produces<AgregarTramiteResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        var group = app.MapGroup("/tramites")
            .WithTags("Trámites")
            .RequireAuthorization();

        group.MapPut("/{id:guid}", (Guid id, ModificarTramiteApiRequest request, ClaimsPrincipal user, ModificarTramiteUseCase useCase) =>
        {
            useCase.Ejecutar(new ModificarTramiteRequest(
                id,
                request.Etiqueta,
                request.Contenido,
                user.ObtenerUserId()));

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", (Guid id, ClaimsPrincipal user, EliminarTramiteUseCase useCase) =>
        {
            useCase.Ejecutar(new EliminarTramiteRequest(id, user.ObtenerUserId()));
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}

public record class AgregarTramiteApiRequest(EtiquetaTramite Etiqueta, string Contenido);

public record class ModificarTramiteApiRequest(EtiquetaTramite Etiqueta, string Contenido);
