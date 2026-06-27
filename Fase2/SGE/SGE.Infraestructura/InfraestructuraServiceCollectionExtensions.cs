using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Tramites;
using SGE.Aplicacion.Usuarios;
using SGE.Infraestructura.Autorizacion;
using SGE.Infraestructura.Datos;
using SGE.Infraestructura.Expedientes;
using SGE.Infraestructura.Seguridad;
using SGE.Infraestructura.Tramites;
using SGE.Infraestructura.Usuarios;

namespace SGE.Infraestructura;

public static class InfraestructuraServiceCollectionExtensions
{
    public static IServiceCollection AddInfraestructura(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<SgeContext>(options => options.UseSqlite(connectionString));

        services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
        services.AddScoped<IExpedienteRepository, ExpedienteRepository>();
        services.AddScoped<ITramiteRepository, TramiteRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAutorizacionService, AutorizacionService>();
        services.AddScoped<IHashService, HashService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
