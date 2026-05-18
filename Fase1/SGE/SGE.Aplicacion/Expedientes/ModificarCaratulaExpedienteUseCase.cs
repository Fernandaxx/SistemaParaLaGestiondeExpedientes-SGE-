using SGE.Aplicacion.Autorizacion;
using SGE.Dominio.Comun;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public class ModificarCaratulaExpedienteUseCase(IExpedienteRepository _repository, IAutorizacionService _autorizacionService)
{
    public ModificarCaratulaExpedienteResponse Ejecutar(ModificarCaratulaExpedienteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteModificacion))
            throw new AutorizacionException("Usuario no autorizado para modificar expedientes.");

        var caratula = new Caratula(request.Caratula);

        var expediente = _repository.ObtenerPorId(request.IdExpediente);
        if (expediente == null)
            throw new DominioException("No se encontró el expediente solicitado.");

        expediente.ModificarCaratula(caratula, request.IdUsuario);

        _repository.Modificar(expediente);

        return new ModificarCaratulaExpedienteResponse();
    }
}