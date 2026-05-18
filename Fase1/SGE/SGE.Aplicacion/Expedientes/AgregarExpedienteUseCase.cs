using SGE.Aplicacion.Autorizacion;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public class AgregarExpedienteUseCase(IExpedienteRepository _repository, IAutorizacionService _autorizacionService)
{
    public AgregarExpedienteResponse Ejecutar(AgregarExpedienteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteAlta))
            throw new AutorizacionException("Usuario no autorizado para agregar expedientes.");

        var caratula = new Caratula(request.Caratula);

        var expediente = new Expediente(caratula, request.IdUsuario);

        _repository.Agregar(expediente);

        return new AgregarExpedienteResponse(expediente.Id);
    }
}