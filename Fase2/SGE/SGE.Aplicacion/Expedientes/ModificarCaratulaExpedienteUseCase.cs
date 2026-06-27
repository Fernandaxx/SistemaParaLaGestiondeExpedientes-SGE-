using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Usuarios;

namespace SGE.Aplicacion.Expedientes;

public class ModificarCaratulaExpedienteUseCase(IExpedienteRepository _repository, IAutorizacionService _autorizacionService, IUnidadDeTrabajo _unidadDeTrabajo)
{
    public ModificarCaratulaExpedienteResponse Ejecutar(ModificarCaratulaExpedienteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteModificacion))
            throw new AutorizacionException("Usuario no autorizado para modificar expedientes.");

        var caratula = new Caratula(request.Caratula);

        var expediente = _repository.ObtenerPorId(request.IdExpediente);
        if (expediente == null)
            throw new EntidadNoEncontradaException("No se encontró el expediente solicitado.");

        expediente.ModificarCaratula(caratula, request.IdUsuario);

        _repository.Modificar(expediente);
        _unidadDeTrabajo.Guardar();

        return new ModificarCaratulaExpedienteResponse();
    }
}
