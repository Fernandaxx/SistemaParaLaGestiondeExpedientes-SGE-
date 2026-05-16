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

    private Tramite(Guid id, Guid expedienteId, Guid idUsuario, EtiquetaTramite etiqueta, ContenidoTramite contenido, DateTime fechaCreacion, DateTime fechaUltimaModificacion)
    {
        if (id == Guid.Empty)
        {
            throw new DominioException("Debe especificarse un identificador válido para reconstruir el trámite.");
        }

        if (expedienteId == Guid.Empty)
        {
            throw new DominioException("El trámite debe estar obligatoriamente asociado a un expediente válido para su reconstrucción.");
        }

        if (idUsuario == Guid.Empty)
        {
            throw new DominioException("Debe especificarse un usuario válido para reconstruir el trámite.");
        }

        if (!Enum.IsDefined(typeof(EtiquetaTramite), etiqueta))
        {
            throw new DominioException("La etiqueta del trámite no es válida para su reconstrucción.");
        }

        if (contenido == null)
        {
            throw new DominioException("El contenido del trámite es obligatorio para su reconstrucción.");
        }

        if (fechaCreacion == default)
        {
            throw new DominioException("La fecha de creación es obligatoria para reconstruir el trámite.");
        }

        if (fechaUltimaModificacion == default)
        {
            throw new DominioException("La fecha de última modificación es obligatoria para reconstruir el trámite.");
        }

        if (fechaUltimaModificacion < fechaCreacion)
        {
            throw new DominioException("La fecha de última modificación no puede ser anterior a la fecha de creación.");
        }

        Id = id;
        ExpedienteId = expedienteId;
        IdUsuario = idUsuario;
        Etiqueta = etiqueta;
        Contenido = contenido;
        FechaCreacion = fechaCreacion;
        FechaUltimaModificacion = fechaUltimaModificacion;
    }

    public static Tramite Reconstruir(Guid id, Guid expedienteId, Guid idUsuario, EtiquetaTramite etiqueta, ContenidoTramite contenido, DateTime fechaCreacion, DateTime fechaUltimaModificacion)
    {
        return new Tramite(id, expedienteId, idUsuario, etiqueta, contenido, fechaCreacion, fechaUltimaModificacion);
    }




}