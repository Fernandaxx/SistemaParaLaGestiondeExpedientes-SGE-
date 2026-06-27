using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Usuarios;

namespace SGE.Aplicacion.Expedientes;

public class CambiarEstadoExpedienteUseCase(IExpedienteRepository _repository, IAutorizacionService _autorizacionService, IUnidadDeTrabajo _unidadDeTrabajo)
{
    public CambiarEstadoExpedienteResponse Ejecutar(CambiarEstadoExpedienteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteModificacion))
            throw new AutorizacionException("Usuario no autorizado para modificar expedientes.");

        var expediente = _repository.ObtenerPorId(request.IdExpediente);
        if (expediente == null)
            throw new EntidadNoEncontradaException("No se encontró el expediente solicitado.");

        expediente.CambiarEstado(request.Estado, request.IdUsuario);

        _repository.Modificar(expediente);
        _unidadDeTrabajo.Guardar();

        return new CambiarEstadoExpedienteResponse();
    }
}
