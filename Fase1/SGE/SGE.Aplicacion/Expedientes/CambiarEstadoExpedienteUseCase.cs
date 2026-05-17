namespace SGE.Aplicacion.Expedientes;
public class CambiarEstadoExpedienteUseCase (IExpedienteRepository _repository) {
    public CambiarEstadoExpedienteResponse Ejecutar(CambiarEstadoExpedienteRequest request)
    {
        if (!_repository.PoseeElPermiso(request.IdUsuario, ExpedienteModificacion))
            throw new AutorizacionException("Usuario no autorizado para modificar expedientes.");

        var expediente = _repository.ObtenerPorId(request.Id);
        expediente.CambiarEstado(request.Estado, request.IdUsuario); // Logica de negocio C

        _repository.Modificar(expediente);

        return new CambiarEstadoExpedienteResponse();
    }
}
