using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion.Expedientes;
public record class ExpedienteDTO(Guid Id, string Caratula, EstadoExpediente Estado);
