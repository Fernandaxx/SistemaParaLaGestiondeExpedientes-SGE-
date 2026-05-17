namespace SGE.Aplicacion.Expedientes;

public class ActualizacionEstadoExpedienteService (IExpedienteRepository _repository, ITramiteRepository _tramiteRepository, IAutorizacionService _autorizacionService) {
    public void ActualizacionEstadoExpedienteService(Guid idExpediente, Guid idUsuario)
    {
        if (!_autorizacionService.PoseeElPermiso(idUsuario, ExpedienteModificacion))
            throw new AutorizacionException("Usuario no autorizado para modificar expedientes.");

        var expediente = _repository.ObtenerPorId(idExpediente);
        var tramites = _tramiteRepository.ListarPorExpediente(idExpediente);
        var ultimoTramite = tramites.LastOrDefault(); // COMO ELIJO EL DE FECHA MAS CERCANA??
        var ultimaEtiqueta = ultimoTramite?.Etiqueta;

        bool cambio = expediente.ActualizarEstado(ultimaEtiqueta, idUsuario); // Logica de negocio B

        if (cambio)
            _repository.Modificar(expediente);

    }
}