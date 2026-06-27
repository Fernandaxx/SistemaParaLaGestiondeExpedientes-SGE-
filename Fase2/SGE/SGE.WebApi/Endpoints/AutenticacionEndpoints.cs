using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Usuarios;

namespace SGE.WebApi.Endpoints;

public static class AutenticacionEndpoints
{
    public static IEndpointRouteBuilder MapAutenticacionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Autenticación");

        group.MapPost("/registrar", (RegistrarUsuarioApiRequest request, RegistrarUsuarioUseCase useCase) =>
        {
            var response = useCase.Ejecutar(new RegistrarUsuarioRequest(
                request.Nombre,
                request.CorreoElectronico,
                request.Contrasena));

            return Results.Created($"/usuarios/{response.Id}", response);
        })
        .AllowAnonymous()
        .Produces<RegistrarUsuarioResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/login", (LoginApiRequest request, LoginUseCase useCase) =>
        {
            var response = useCase.Ejecutar(new LoginRequest(
                request.CorreoElectronico,
                request.Contrasena));

            return Results.Ok(response);
        })
        .AllowAnonymous()
        .Produces<LoginResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        return app;
    }
}

public record class RegistrarUsuarioApiRequest(string Nombre, string CorreoElectronico, string Contrasena);

public record class LoginApiRequest(string CorreoElectronico, string Contrasena);
