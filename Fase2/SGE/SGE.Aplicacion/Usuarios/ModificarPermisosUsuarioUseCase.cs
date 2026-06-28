using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Usuarios;

public class ModificarPermisosUsuarioUseCase(IUsuarioRepository _repository, IAutorizacionService _autorizacionService, IUnidadDeTrabajo _unidadDeTrabajo)
{
    public ModificarPermisosUsuarioResponse Ejecutar(ModificarPermisosUsuarioRequest request)
    {
        if (!_autorizacionService.EsAdministrador(request.IdUsuarioEjecutor))
            throw new AutorizacionException("Usuario no autorizado para gestionar usuarios.");

        var usuario = _repository.ObtenerPorId(request.IdUsuario);
        if (usuario is null)
            throw new EntidadNoEncontradaException("No se encontró el usuario solicitado.");

        usuario.ReemplazarPermisos(request.Permisos);

        _repository.Modificar(usuario);
        _unidadDeTrabajo.Guardar();

        return new ModificarPermisosUsuarioResponse();
    }
}