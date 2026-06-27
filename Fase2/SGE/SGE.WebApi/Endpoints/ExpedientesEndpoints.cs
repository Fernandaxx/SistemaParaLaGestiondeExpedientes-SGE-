using System.Security.Claims;
using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Expedientes;
using SGE.WebApi.Comun;

namespace SGE.WebApi.Endpoints;

public static class ExpedientesEndpoints
{
    public static IEndpointRouteBuilder MapExpedientesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/expedientes")
            .WithTags("Expedientes")
            .RequireAuthorization();

        group.MapGet("", (ListarTodosLosExpedientesUseCase useCase) =>
        {
            var response = useCase.Ejecutar(new ListarTodosLosExpedientesRequest());
            return Results.Ok(response);
        })
        .Produces<ListarTodosLosExpedientesResponse>();

        group.MapGet("/{id:guid}", (Guid id, ObtenerExpedientePorIdUseCase useCase) =>
        {
            var response = useCase.Ejecutar(new ObtenerExpedientePorIdRequest(id));
            return Results.Ok(response);
        })
        .Produces<ObtenerExpedientePorIdResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("", (AgregarExpedienteApiRequest request, ClaimsPrincipal user, AgregarExpedienteUseCase useCase) =>
        {
            var response = useCase.Ejecutar(new AgregarExpedienteRequest(
                request.Caratula,
                user.ObtenerUserId()));

            return Results.Created($"/expedientes/{response.Id}", response);
        })
        .Produces<AgregarExpedienteResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        group.MapPut("/{id:guid}/caratula", (Guid id, ModificarCaratulaExpedienteApiRequest request, ClaimsPrincipal user, ModificarCaratulaExpedienteUseCase useCase) =>
        {
            useCase.Ejecutar(new ModificarCaratulaExpedienteRequest(
                id,
                request.Caratula,
                user.ObtenerUserId()));

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}/estado", (Guid id, CambiarEstadoExpedienteApiRequest request, ClaimsPrincipal user, CambiarEstadoExpedienteUseCase useCase) =>
        {
            useCase.Ejecutar(new CambiarEstadoExpedienteRequest(
                id,
                request.Estado,
                user.ObtenerUserId()));

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", (Guid id, ClaimsPrincipal user, EliminarExpedienteUseCase useCase) =>
        {
            useCase.Ejecutar(new EliminarExpedienteRequest(id, user.ObtenerUserId()));
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}

public record class AgregarExpedienteApiRequest(string Caratula);

public record class ModificarCaratulaExpedienteApiRequest(string Caratula);

public record class CambiarEstadoExpedienteApiRequest(EstadoExpediente Estado);
