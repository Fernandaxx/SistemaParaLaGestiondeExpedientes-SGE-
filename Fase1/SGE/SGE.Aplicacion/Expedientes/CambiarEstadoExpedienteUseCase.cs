using SGE.Aplicacion.Autorizacion;
using SGE.Dominio.Comun;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public class CambiarEstadoExpedienteUseCase(IExpedienteRepository _repository, IAutorizacionService _autorizacionService)
{
    public CambiarEstadoExpedienteResponse Ejecutar(CambiarEstadoExpedienteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteModificacion))
            throw new AutorizacionException("Usuario no autorizado para modificar expedientes.");

        var expediente = _repository.ObtenerPorId(request.IdExpediente);
        if (expediente == null)
            throw new DominioException("No se encontró el expediente solicitado.");

        expediente.CambiarEstado(request.Estado, request.IdUsuario);

        _repository.Modificar(expediente);

        return new CambiarEstadoExpedienteResponse();
    }
}