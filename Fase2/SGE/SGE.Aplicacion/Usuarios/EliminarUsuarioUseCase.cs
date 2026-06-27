using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Usuarios;

public class EliminarUsuarioUseCase(IUsuarioRepository _repository, IUnidadDeTrabajo _unidadDeTrabajo)
{
    public EliminarUsuarioResponse Ejecutar(EliminarUsuarioRequest request)
    {
        VerificarAdministrador(request.IdUsuarioEjecutor);

        var usuario = _repository.ObtenerPorId(request.IdUsuario);
        if (usuario is null)
            throw new EntidadNoEncontradaException("No se encontró el usuario solicitado.");

        _repository.Eliminar(usuario);
        _unidadDeTrabajo.Guardar();

        return new EliminarUsuarioResponse();
    }

    private void VerificarAdministrador(Guid idUsuarioEjecutor)
    {
        var usuarioEjecutor = _repository.ObtenerPorId(idUsuarioEjecutor);
        if (usuarioEjecutor is null || !usuarioEjecutor.EsAdministrador)
            throw new AutorizacionException("Usuario no autorizado para gestionar usuarios.");
    }
}
