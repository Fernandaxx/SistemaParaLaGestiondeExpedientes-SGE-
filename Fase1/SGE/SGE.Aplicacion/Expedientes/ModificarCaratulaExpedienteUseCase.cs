namespace SGE.Aplicacion.Expedientes;
public class ModificarCaratulaExpedienteUseCase (IExpedienteRepository _repository) {
    public ModificarCaratulaExpedienteResponse Ejecutar(ModificarCaratulaExpedienteRequest request)
    {
        if (!_repository.PoseeElPermiso(request.IdUsuario, ExpedienteModificacion))
            throw new AutorizacionException("Usuario no autorizado para modificar expedientes.");

        var caratula = new Caratula(request.Caratula);

        var expediente = _repository.ObtenerPorId(request.Id);
        expediente.ModificarCaratula(caratula, request.IdUsuario); // Logica de negocio A

        _repository.Modificar(expediente);

        return new ModificarCaratulaExpedienteResponse();
    }
}