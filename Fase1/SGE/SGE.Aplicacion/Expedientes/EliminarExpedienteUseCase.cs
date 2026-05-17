namespace SGE.Aplicacion.Expedientes;
public class EliminarExpedienteUseCase (IExpedienteRepository _repository, ITramiteRepository _tramiteRepository) {
    public EliminarExpedienteResponse Ejecutar(EliminarExpedienteRequest request)
    {
        var expediente = _repository.ObtenerPorId(request.Id);

        if (!_repository.PoseeElPermiso(request.IdUsuario, ExpedienteBaja))
            throw new AutorizacionException("Usuario no autorizado para eliminar expedientes.");

        if (expediente is null)
            throw new DominioException("Entidad no encontrada. No se puede eliminar.");

        // Eliminar tramites antes de eliminar el expediente
        IEnumerable<Tramite> tramites = _tramiteRepository.ListarPorExpediente(request.Id);
        foreach (var tramite in tramites)
        {
            _tramiteRepository.Eliminar(tramite);
        }

        // Eliminar expediente
        _repository.Eliminar(expediente);

        return new EliminarExpedienteResponse();
    }
}