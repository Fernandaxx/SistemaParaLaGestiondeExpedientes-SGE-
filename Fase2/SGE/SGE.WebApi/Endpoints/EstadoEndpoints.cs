namespace SGE.WebApi.Endpoints;

public static class EstadoEndpoints
{
    public static IEndpointRouteBuilder MapEstadoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => "SGE WebApi funcionando.")
            .WithTags("Estado");

        return app;
    }
}
