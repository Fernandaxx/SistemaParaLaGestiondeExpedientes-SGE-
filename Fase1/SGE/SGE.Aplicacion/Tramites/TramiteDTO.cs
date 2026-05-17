namespace SGE.Aplicacion.Tramites;
public record class TramiteDTO(Guid Id, Guid ExpedienteId, EtiquetaTramite Etiqueta, string Contenido);
