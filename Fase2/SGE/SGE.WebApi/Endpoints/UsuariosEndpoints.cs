using System.Security.Claims;
using SGE.Aplicacion.Usuarios;
using SGE.Dominio.Usuarios;
using SGE.WebApi.Comun;

namespace SGE.WebApi.Endpoints;

public static class UsuariosEndpoints
{
    public static IEndpointRouteBuilder MapUsuariosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/usuarios")
            .WithTags("Usuarios")
            .RequireAuthorization();

        group.MapGet("", (ClaimsPrincipal user, ListarUsuariosUseCase useCase) =>
        {
            var response = useCase.Ejecutar(new ListarUsuariosRequest(user.ObtenerUserId()));
            return Results.Ok(response);
        })
        .Produces<ListarUsuariosResponse>()
        .ProducesProblem(StatusCodes.Status403Forbidden);

        group.MapPut("/me", (ModificarMisDatosApiRequest request, ClaimsPrincipal user, ModificarMisDatosUseCase useCase) =>
        {
            var userId = user.ObtenerUserId();
            useCase.Ejecutar(new ModificarMisDatosRequest(
                userId,
                userId,
                request.Nombre,
                request.CorreoElectronico,
                request.Contrasena));

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", (Guid id, ClaimsPrincipal user, EliminarUsuarioUseCase useCase) =>
        {
            useCase.Ejecutar(new EliminarUsuarioRequest(user.ObtenerUserId(), id));
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}/permisos", (Guid id, ModificarPermisosUsuarioApiRequest request, ClaimsPrincipal user, ModificarPermisosUsuarioUseCase useCase) =>
        {
            useCase.Ejecutar(new ModificarPermisosUsuarioRequest(
                user.ObtenerUserId(),
                id,
                request.Permisos));

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}

public record class ModificarMisDatosApiRequest(string? Nombre, string? CorreoElectronico, string? Contrasena);

public record class ModificarPermisosUsuarioApiRequest(IEnumerable<Permiso> Permisos);
