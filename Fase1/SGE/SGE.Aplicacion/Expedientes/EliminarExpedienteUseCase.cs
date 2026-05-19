using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Tramites;
using SGE.Dominio.Comun;
using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Expedientes;

public class EliminarExpedienteUseCase(IExpedienteRepository _repository, ITramiteRepository _tramiteRepository, IAutorizacionService _autorizacionService)
{
    public EliminarExpedienteResponse Ejecutar(EliminarExpedienteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteBaja))
            throw new AutorizacionException("Usuario no autorizado para eliminar expedientes.");

        var expediente = _repository.ObtenerPorId(request.Id);

        if (expediente is null)
            throw new DominioException("Entidad no encontrada. No se puede eliminar.");

        IEnumerable<Tramite> tramites = _tramiteRepository.ListarPorExpediente(request.Id);
        foreach (var tramite in tramites) {
            _tramiteRepository.Eliminar(tramite);
        }

        _repository.Eliminar(expediente);

        return new EliminarExpedienteResponse();
    }
}