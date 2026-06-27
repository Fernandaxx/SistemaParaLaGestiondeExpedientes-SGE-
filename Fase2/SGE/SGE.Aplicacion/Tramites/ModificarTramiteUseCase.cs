using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Usuarios;


namespace SGE.Aplicacion.Tramites;

public class ModificarTramiteUseCase(ITramiteRepository _repository, IAutorizacionService _autorizacionService, ActualizacionEstadoExpedienteService _actualizacionService, IUnidadDeTrabajo _unidadDeTrabajo)
{
    public ModificarTramiteResponse Ejecutar(ModificarTramiteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.TramiteModificacion))
            throw new AutorizacionException("Usuario no autorizado para modificar trámites.");

        var tramite = _repository.ObtenerPorId(request.Id);
        
        if (tramite == null) {
            throw new EntidadNoEncontradaException("No se encontró el trámite solicitado.");
        }

        tramite.Modificar(request.Etiqueta, request.Contenido, request.IdUsuario);
        _repository.Modificar(tramite);
        
        _actualizacionService.ActualizarEstadoExpediente(tramite.ExpedienteId, request.IdUsuario);
        _unidadDeTrabajo.Guardar();

        return new ModificarTramiteResponse();
    }
}
