using SGE.Aplicacion.Tramites;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public record class ExpedienteDetalleDTO(
    Guid Id,
    string Caratula,
    EstadoExpediente Estado,
    DateTime FechaCreacion,
    DateTime FechaModificacion,
    Guid UsuarioUltimoCambio,
    IEnumerable<TramiteDetalleDTO> Tramites);
