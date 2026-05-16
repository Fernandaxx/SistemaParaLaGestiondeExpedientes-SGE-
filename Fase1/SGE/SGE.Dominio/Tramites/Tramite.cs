using SGE.Dominio.Comun;

namespace SGE.Dominio.Tramites;

public class Tramite
{

    public Guid Id { get; private set; }
    public Guid ExpedienteId { get; private set; }
    public Guid IdUsuario { get; private set; }
    public EtiquetaTramite Etiqueta { get; private set; }
    public ContenidoTramite Contenido { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaUltimaModificacion { get; private set; }


    public Tramite(Guid expedienteId, Guid idUsuario, EtiquetaTramite etiqueta, ContenidoTramite contenido)
    {
        if (expedienteId == Guid.Empty)
        {
            throw new DominioException("El trámite debe estar obligatoriamente asociado a un expediente válido.");
        }

        if (idUsuario == Guid.Empty)
        {
            throw new DominioException("Debe especificarse un usuario válido para registrar la autoría del trámite.");
        }

        if (contenido == null)
        {
            throw new DominioException("El contenido del trámite es obligatorio.");
        }

        Id = Guid.NewGuid();
        ExpedienteId = expedienteId;
        IdUsuario = idUsuario;
        Etiqueta = etiqueta;
        Contenido = contenido;

        DateTime ahora = DateTime.Now;
        FechaCreacion = ahora;
        FechaUltimaModificacion = ahora;
    }


}