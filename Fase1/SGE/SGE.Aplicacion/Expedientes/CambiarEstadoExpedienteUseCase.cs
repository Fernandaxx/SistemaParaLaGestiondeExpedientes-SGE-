namespace SGE.Aplicacion.Expedientes;
public class CambiarEstadoExpedienteUseCase (IExpedienteRepository _repository, IAutorizacionService _autorizacionService) {
    public CambiarEstadoExpedienteResponse Ejecutar(CambiarEstadoExpedienteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, ExpedienteModificacion))
            throw new AutorizacionException("Usuario no autorizado para modificar expedientes.");

        var expediente = _repository.ObtenerPorId(request.Id);
        expediente.CambiarEstado(request.Estado, request.IdUsuario); // Logica de negocio C

        _repository.Modificar(expediente);

        return new CambiarEstadoExpedienteResponse();
    }
}
