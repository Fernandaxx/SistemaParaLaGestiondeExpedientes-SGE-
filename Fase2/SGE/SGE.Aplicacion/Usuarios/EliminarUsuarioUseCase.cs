using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Usuarios;

public class EliminarUsuarioUseCase(IUsuarioRepository _repository, IAutorizacionService _autorizacionService, IUnidadDeTrabajo _unidadDeTrabajo)
{
    public EliminarUsuarioResponse Ejecutar(EliminarUsuarioRequest request)
    {
        if (!_autorizacionService.EsAdministrador(request.IdUsuarioEjecutor))
            throw new AutorizacionException("Usuario no autorizado para gestionar usuarios.");

        var usuario = _repository.ObtenerPorId(request.IdUsuario);
        if (usuario is null)
            throw new EntidadNoEncontradaException("No se encontró el usuario solicitado.");

        _repository.Eliminar(usuario);
        _unidadDeTrabajo.Guardar();

        return new EliminarUsuarioResponse();
    }
}