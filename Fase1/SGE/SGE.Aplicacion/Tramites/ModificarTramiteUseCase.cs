namespace SGE.Aplicacion.Tramites;
public class ModificarTramiteUseCase (ITramiteRepository _repository, IAutorizacionService _autorizacionService, ActualizacionEstadoExpedienteService _actualizacionService) {
    public ModificarTramiteResponse Ejecutar(ModificarTramiteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, TramiteModificacion))
            throw new AutorizacionException("Usuario no autorizado para modificar trámites.");

        var contenido = new ContenidoTramite(request.Contenido);

        var tramite = _repository.ObtenerPorId(request.Id);
        tramite.Modificar(request.Etiqueta, request.Contenido);

        _repository.Modificar(tramite);
        
        // Actualizar estado del expediente
        _actualizacionService.ActualizacionEstadoExpedienteService(request.IdExpediente, request.IdUsuario);

        return new ModificarTramiteResponse();
    }
}