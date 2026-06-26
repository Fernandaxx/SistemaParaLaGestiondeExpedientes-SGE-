using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Tramites;

public class AgregarTramiteUseCase(ITramiteRepository _repository, IAutorizacionService _autorizacionService, ActualizacionEstadoExpedienteService _actualizacionService)
{
    public AgregarTramiteResponse Ejecutar(AgregarTramiteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.TramiteAlta))
            throw new AutorizacionException("Usuario no autorizado para agregar trámites.");

        var contenido = new ContenidoTramite(request.Contenido);
        var tramite = new Tramite(request.ExpedienteId, request.IdUsuario, request.Etiqueta, contenido);

        _repository.Agregar(tramite);

        _actualizacionService.ActualizarEstadoExpediente(request.ExpedienteId, request.IdUsuario);

        return new AgregarTramiteResponse(tramite.Id);
    }
}