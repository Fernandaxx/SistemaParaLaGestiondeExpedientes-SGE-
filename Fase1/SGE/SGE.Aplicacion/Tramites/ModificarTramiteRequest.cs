namespace SGE.Aplicacion.Tramites;
public record class ModificarTramiteRequest(Guid Id, EtiquetaTramite Etiqueta, string Contenido, Guid IdUsuario);