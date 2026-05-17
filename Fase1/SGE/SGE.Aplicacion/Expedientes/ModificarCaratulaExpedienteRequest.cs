namespace SGE.Aplicacion.Expedientes;
public record class ModificarCaratulaExpedienteRequest(Guid IdExpediente, string Caratula, Guid IdUsuario);
    