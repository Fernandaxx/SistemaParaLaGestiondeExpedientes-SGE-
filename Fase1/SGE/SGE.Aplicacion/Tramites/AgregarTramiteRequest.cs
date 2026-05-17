namespace SGE.Aplicacion.Tramites;
public record class AgregarTramiteRequest(Guid ExpedienteId, EtiquetaTramite Etiqueta, string Contenido, Guid IdUsuario);