namespace SGE.Aplicacion.Tramites;
public class EliminarTramiteUseCase (ITramiteRepository _repository, IAutorizacionService _autorizacionService, ActualizacionEstadoExpedienteService _actualizacionService) {
    public EliminarTramiteResponse Ejecutar(EliminarTramiteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, TramiteBaja))
            throw new AutorizacionException("Usuario no autorizado para eliminar tramites.");

        var tramite = _repository.ObtenerPorId(request.Id);

        if (tramite is null)
            throw new DominioException("Entidad no encontrada. No se puede eliminar.");
        
        _repository.Eliminar(tramite);

        // Actualizar estado del expediente
        _actualizacionService.ActualizacionEstadoExpedienteService(request.IdExpediente, request.IdUsuario);

        return new EliminarTramiteResponse();
    }
}