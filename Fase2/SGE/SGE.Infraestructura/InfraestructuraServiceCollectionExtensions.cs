using Microsoft.Extensions.DependencyInjection;
using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Tramites;
using SGE.Infraestructura.Autorizacion;
using SGE.Infraestructura.Expedientes;
using SGE.Infraestructura.Tramites;

namespace SGE.Infraestructura;

public static class InfraestructuraServiceCollectionExtensions
{
    public static IServiceCollection AddInfraestructura(this IServiceCollection services)
    {
        services.AddScoped<IExpedienteRepository, ExpedienteTxtRepository>();
        services.AddScoped<ITramiteRepository, TramiteTxtRepository>();
        services.AddScoped<IAutorizacionService, AutorizacionProvisionalService>();

        return services;
    }
}
