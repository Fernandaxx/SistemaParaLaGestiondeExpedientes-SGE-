using System.Data;
using Microsoft.EntityFrameworkCore;
using SGE.Aplicacion.Autorizacion;
using SGE.Dominio.Usuarios;

namespace SGE.Infraestructura.Datos;

public static class SgeSqlite
{
    public static void Inicializar(SgeContext context, IHashService hashService)
    {
        context.Database.EnsureCreated();

        var connection = context.Database.GetDbConnection();
        var debeCerrarConexion = connection.State != ConnectionState.Open;

        if (debeCerrarConexion) {
            connection.Open();
        }

        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA journal_mode=DELETE;";
        command.ExecuteNonQuery();

        if (debeCerrarConexion) {
            connection.Close();
        }

        CargarUsuariosSemilla(context, hashService);
    }

    private static void CargarUsuariosSemilla(SgeContext context, IHashService hashService)
    {
        if (context.Usuarios.Any()) return;

        var admin = new Usuario(
            "Administrador SGE",
            "admin@sge.com",
            hashService.GenerarHash("admin123"),
            esAdministrador: true);

        var usuarioConPermisos = new Usuario(
            "Usuario con permisos",
            "operador@sge.com",
            hashService.GenerarHash("operador123"));
        usuarioConPermisos.AsignarPermiso(Permiso.ExpedienteAlta);
        usuarioConPermisos.AsignarPermiso(Permiso.ExpedienteModificacion);
        usuarioConPermisos.AsignarPermiso(Permiso.TramiteAlta);
        usuarioConPermisos.AsignarPermiso(Permiso.TramiteModificacion);

        var usuarioSinPermisos = new Usuario(
            "Usuario sin permisos",
            "lector@sge.com",
            hashService.GenerarHash("lector123"));

        context.Usuarios.Add(admin);
        context.Usuarios.Add(usuarioConPermisos);
        context.Usuarios.Add(usuarioSinPermisos);
        context.SaveChanges();

        var usuarioPermisos = context.Set<Dictionary<string, object>>("UsuarioPermisos");
        foreach (var permiso in usuarioConPermisos.Permisos) {
            usuarioPermisos.Add(new Dictionary<string, object>
            {
                ["UsuarioId"] = usuarioConPermisos.Id,
                ["Permiso"] = permiso
            });
        }

        context.SaveChanges();
    }
}
