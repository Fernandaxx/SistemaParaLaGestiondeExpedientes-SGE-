using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Comun;

namespace SGE.Aplicacion.Tramites;

public class EliminarTramiteUseCase(ITramiteRepository _repository, IAutorizacionService _autorizacionService, ActualizacionEstadoExpedienteService _actualizacionService)
{
    public EliminarTramiteResponse Ejecutar(EliminarTramiteRequest request)
    {
        // fix 1: agrego "Permiso."
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.TramiteBaja))
            throw new AutorizacionException("Usuario no autorizado para eliminar tramites.");

        var tramite = _repository.ObtenerPorId(request.Id);

        if (tramite is null)
            throw new DominioException("Entidad no encontrada. No se puede eliminar.");

        _repository.Eliminar(tramite);

        // fix 2 y 3: Sacamos el ID del expediente desde la entidad 'tramite' y llamamos a 'ActualizarEstadoExpediente'
        _actualizacionService.ActualizarEstadoExpediente(tramite.ExpedienteId, request.IdUsuario);

        return new EliminarTramiteResponse();
    }
}