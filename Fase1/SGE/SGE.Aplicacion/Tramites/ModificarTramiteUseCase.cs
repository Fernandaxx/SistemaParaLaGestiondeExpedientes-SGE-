namespace SGE.Aplicacion.Tramites;
public class ModificarTramiteUseCase (ITramiteRepository _repository) {
    public ModificarTramiteResponse Ejecutar(ModificarTramiteRequest request)
    {
        if (!_repository.PoseeElPermiso(request.IdUsuario, TramiteModificacion))
            throw new AutorizacionException("Usuario no autorizado para modificar trámites.");

        var contenido = new ContenidoTramite(request.Contenido);

        var tramite = _repository.ObtenerPorId(request.Id);
        tramite.Modificar(request.Etiqueta, request.Contenido);

        _repository.Modificar(tramite);
        
        // Actualizar estado del expediente
        ActualizacionEstadoExpedienteService(request.IdExpediente, request.IdUsuario);

        return new ModificarTramiteResponse();
    }
}