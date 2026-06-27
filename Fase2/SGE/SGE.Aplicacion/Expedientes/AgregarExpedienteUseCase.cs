using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Usuarios;

namespace SGE.Aplicacion.Expedientes;

public class AgregarExpedienteUseCase(IExpedienteRepository _repository, IAutorizacionService _autorizacionService, IUnidadDeTrabajo _unidadDeTrabajo)
{
    public AgregarExpedienteResponse Ejecutar(AgregarExpedienteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteAlta))
            throw new AutorizacionException("Usuario no autorizado para agregar expedientes.");

        var caratula = new Caratula(request.Caratula);

        var expediente = new Expediente(caratula, request.IdUsuario);

        _repository.Agregar(expediente);
        _unidadDeTrabajo.Guardar();

        return new AgregarExpedienteResponse(expediente.Id);
    }
}
