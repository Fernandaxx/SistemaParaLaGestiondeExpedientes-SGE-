namespace SGE.Aplicacion.Tramites;
public class EliminarTramiteUseCase (ITramiteRepository _repository) {
    public EliminarTramiteResponse Ejecutar(EliminarTramiteRequest request)
    {
        var tramite = _repository.ObtenerPorId(request.Id);

        if (!_repository.PoseeElPermiso(request.IdUsuario, TramiteBaja))
            throw new AutorizacionException("Usuario no autorizado para eliminar tramites.");

        if (tramite is null)
            throw new DominioException("Entidad no encontrada. No se puede eliminar.");
        
        _repository.Eliminar(tramite);

        // Actualizar estado del expediente
        ActualizacionEstadoExpedienteService(request.IdExpediente, request.IdUsuario);

        return new EliminarTramiteResponse();
    }
}