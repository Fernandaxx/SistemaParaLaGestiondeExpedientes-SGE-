using SGE.Aplicacion.Autorizacion;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public class AgregarExpedienteUseCase(IExpedienteRepository _repository, IAutorizacionService _autorizacionService)
{
    public AgregarExpedienteResponse Ejecutar(AgregarExpedienteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, ExpedienteAlta))
            throw new AutorizacionException("Usuario no autorizado para agregar expedientes.");

        // Los Value Objects se encargan de las validaciones de formato/rango
        var caratula = new Caratula(request.Caratula);

        // La Entidad nace con su identidad (Guid) de forma autónoma
        var expediente = new Expediente(caratula, request.IdUsuario);

        // Persistencia
        _repository.Agregar(expediente);

        // Salida
        return new AgregarExpedienteResponse(expediente.Id);
    }
}