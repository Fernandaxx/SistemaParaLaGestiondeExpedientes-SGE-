using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Usuarios;
using SGE.Dominio.Usuarios;

namespace SGE.Infraestructura.Autorizacion;

public class AutorizacionService(IUsuarioRepository _usuarioRepository) : IAutorizacionService
{
    public bool PoseeElPermiso(Guid idUsuario, Permiso permiso)
    {
        var usuario = _usuarioRepository.ObtenerPorId(idUsuario);
        return usuario?.PoseePermiso(permiso) ?? false;
    }
}
