using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Aplicacion.Tramites;
using SGE.Dominio.Tramites;
using SGE.Dominio.Usuarios;

namespace SGE.Aplicacion.Expedientes;

public class EliminarExpedienteUseCase(IExpedienteRepository _repository, ITramiteRepository _tramiteRepository, IAutorizacionService _autorizacionService, IUnidadDeTrabajo _unidadDeTrabajo)
{
    public EliminarExpedienteResponse Ejecutar(EliminarExpedienteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteBaja))
            throw new AutorizacionException("Usuario no autorizado para eliminar expedientes.");

        var expediente = _repository.ObtenerPorId(request.Id);

        if (expediente is null)
            throw new EntidadNoEncontradaException("Entidad no encontrada. No se puede eliminar.");

        IEnumerable<Tramite> tramites = _tramiteRepository.ListarPorExpediente(request.Id);
        foreach (var tramite in tramites) {
            _tramiteRepository.Eliminar(tramite);
        }

        _repository.Eliminar(expediente);
        _unidadDeTrabajo.Guardar();

        return new EliminarExpedienteResponse();
    }
}
