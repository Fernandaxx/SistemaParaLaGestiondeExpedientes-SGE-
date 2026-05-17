using SGE.Aplicacion.Autorizacion;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public class ModificarCaratulaExpedienteUseCase(IExpedienteRepository _repository, IAutorizacionService _autorizacionService)
{
    public ModificarCaratulaExpedienteResponse Ejecutar(ModificarCaratulaExpedienteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, ExpedienteModificacion))
            throw new AutorizacionException("Usuario no autorizado para modificar expedientes.");

        var caratula = new Caratula(request.Caratula);

        var expediente = _repository.ObtenerPorId(request.Id);
        expediente.ModificarCaratula(caratula, request.IdUsuario); // Logica de negocio A

        _repository.Modificar(expediente);

        return new ModificarCaratulaExpedienteResponse();
    }
}