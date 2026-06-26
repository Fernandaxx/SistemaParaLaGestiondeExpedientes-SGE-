using Microsoft.Extensions.DependencyInjection;
using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Tramites;

namespace SGE.Aplicacion;

public static class AplicacionServiceCollectionExtensions
{
    public static IServiceCollection AddAplicacion(this IServiceCollection services)
    {
        services.AddScoped<ActualizacionEstadoExpedienteService>();

        services.AddScoped<AgregarExpedienteUseCase>();
        services.AddScoped<CambiarEstadoExpedienteUseCase>();
        services.AddScoped<EliminarExpedienteUseCase>();
        services.AddScoped<ListarTodosLosExpedientesUseCase>();
        services.AddScoped<ModificarCaratulaExpedienteUseCase>();

        services.AddScoped<AgregarTramiteUseCase>();
        services.AddScoped<EliminarTramiteUseCase>();
        services.AddScoped<ListarTramitesPorExpedienteUseCase>();
        services.AddScoped<ModificarTramiteUseCase>();

        return services;
    }
}
