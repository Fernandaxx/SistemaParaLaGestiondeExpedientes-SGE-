using SGE.Aplicacion.Expedientes;
namespace SGE.Aplicacion.Tramites;
public class AgregarTramiteUseCase (ITramiteRepository _repository) {
    public AgregarTramiteResponse Ejecutar(AgregarTramiteRequest request)
    {
        if (!_repository.PoseeElPermiso(request.IdUsuario, TramiteAlta))
            throw new AutorizacionException("Usuario no autorizado para agregar trámites.");

        // Los Value Objects se encargan de las validaciones de formato/rango
        var contenido = new ContenidoTramite(request.Contenido);

        // La Entidad nace con su identidad (Guid) de forma autónoma
        var tramite = new Tramite(request.IdExpediente, request.IdUsuario, request.Etiqueta, contenido);

        // Persistencia
        _repository.Agregar(tramite);

        // Actualizar estado del expediente
        ActualizacionEstadoExpedienteService(request.IdExpediente, request.IdUsuario);

        // Salida
        return new AgregarTramiteResponse(tramite.Id);
    }
}