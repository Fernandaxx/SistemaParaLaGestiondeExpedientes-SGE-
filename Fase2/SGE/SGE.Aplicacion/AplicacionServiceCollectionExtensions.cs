using Microsoft.Extensions.DependencyInjection;
using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Tramites;
using SGE.Aplicacion.Usuarios;

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
        services.AddScoped<ObtenerExpedientePorIdUseCase>();

        services.AddScoped<AgregarTramiteUseCase>();
        services.AddScoped<EliminarTramiteUseCase>();
        services.AddScoped<ListarTramitesPorExpedienteUseCase>();
        services.AddScoped<ModificarTramiteUseCase>();

        services.AddScoped<LoginUseCase>();
        services.AddScoped<RegistrarUsuarioUseCase>();
        services.AddScoped<ModificarMisDatosUseCase>();
        services.AddScoped<ListarUsuariosUseCase>();
        services.AddScoped<EliminarUsuarioUseCase>();
        services.AddScoped<ModificarPermisosUsuarioUseCase>();

        return services;
    }
}
