using Microsoft.EntityFrameworkCore;
using SGE.Aplicacion.Usuarios;
using SGE.Dominio.Usuarios;
using SGE.Infraestructura.Datos;

namespace SGE.Infraestructura.Usuarios;

public class UsuarioRepository(SgeContext _context) : IUsuarioRepository
{
    public void Agregar(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        AgregarPermisos(usuario);
    }

    public void Modificar(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        SincronizarPermisos(usuario);
    }

    public void Eliminar(Usuario usuario)
    {
        _context.Usuarios.Remove(usuario);
    }

    public Usuario? ObtenerPorId(Guid id)
    {
        var usuario = _context.Usuarios.Find(id);
        return usuario is null ? null : CargarPermisos(usuario);
    }

    public Usuario? ObtenerPorCorreoElectronico(string correoElectronico)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.CorreoElectronico == correoElectronico);
        return usuario is null ? null : CargarPermisos(usuario);
    }

    public IEnumerable<Usuario> ListarTodos()
    {
        return _context.Usuarios
            .ToList()
            .Select(CargarPermisos)
            .ToList();
    }

    private Usuario CargarPermisos(Usuario usuario)
    {
        var permisos = UsuarioPermisos()
            .Where(p => EF.Property<Guid>(p, "UsuarioId") == usuario.Id)
            .Select(p => EF.Property<Permiso>(p, "Permiso"))
            .ToList();

        usuario.ReemplazarPermisos(permisos);
        return usuario;
    }

    private void SincronizarPermisos(Usuario usuario)
    {
        var permisosActuales = UsuarioPermisos()
            .Where(p => EF.Property<Guid>(p, "UsuarioId") == usuario.Id)
            .ToList();

        UsuarioPermisos().RemoveRange(permisosActuales);
        AgregarPermisos(usuario);
    }

    private void AgregarPermisos(Usuario usuario)
    {
        foreach (var permiso in usuario.Permisos) {
            UsuarioPermisos().Add(new Dictionary<string, object>
            {
                ["UsuarioId"] = usuario.Id,
                ["Permiso"] = permiso
            });
        }
    }

    private DbSet<Dictionary<string, object>> UsuarioPermisos()
    {
        return _context.Set<Dictionary<string, object>>("UsuarioPermisos");
    }
}
