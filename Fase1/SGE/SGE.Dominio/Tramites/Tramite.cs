using SGE.Dominio.Comun;

namespace SGE.Dominio.Tramites;

public class Tramite
{
    public Guid Id { get; private set; }
    public Guid ExpedienteId { get; private set; }
    public Guid UsuarioUltimoCambio { get; private set; } // Renombrado
    public EtiquetaTramite Etiqueta { get; private set; }
    public ContenidoTramite Contenido { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaUltimaModificacion { get; private set; }

    public Tramite(Guid expedienteId, Guid usuarioUltimoCambio, EtiquetaTramite etiqueta, ContenidoTramite contenido)
    {
        if (expedienteId == Guid.Empty) throw new DominioException("Expediente inválido.");
        if (usuarioUltimoCambio == Guid.Empty) throw new DominioException("Usuario inválido.");
        if (contenido == null) throw new DominioException("Contenido obligatorio.");

        Id = Guid.NewGuid();
        ExpedienteId = expedienteId;
        UsuarioUltimoCambio = usuarioUltimoCambio;
        Etiqueta = etiqueta;
        Contenido = contenido;
        DateTime ahora = DateTime.Now;
        FechaCreacion = ahora;
        FechaUltimaModificacion = ahora;
    }

    private Tramite(Guid id, Guid expedienteId, Guid usuarioUltimoCambio, EtiquetaTramite etiqueta, ContenidoTramite contenido, DateTime fechaCreacion, DateTime fechaUltimaModificacion)
    {
        if (id == Guid.Empty) throw new DominioException("ID inválido.");
        if (expedienteId == Guid.Empty) throw new DominioException("Expediente inválido.");
        if (usuarioUltimoCambio == Guid.Empty) throw new DominioException("Usuario inválido.");
        if (contenido == null) throw new DominioException("Contenido obligatorio.");
        if (fechaCreacion == default) throw new DominioException("La fecha de creación es obligatoria para reconstruir el trámite.");
        if (fechaUltimaModificacion == default) throw new DominioException("La fecha de última modificación es obligatoria para reconstruir el trámite.");
        if (fechaUltimaModificacion < fechaCreacion) throw new DominioException("La fecha de última modificación no puede ser anterior a la fecha de creación.");

        Id = id;
        ExpedienteId = expedienteId;
        UsuarioUltimoCambio = usuarioUltimoCambio;
        Etiqueta = etiqueta;
        Contenido = contenido;
        FechaCreacion = fechaCreacion;
        FechaUltimaModificacion = fechaUltimaModificacion;
    }

    public static Tramite Reconstruir(Guid id, Guid expedienteId, Guid usuarioUltimoCambio, EtiquetaTramite etiqueta, ContenidoTramite contenido, DateTime fechaCreacion, DateTime fechaUltimaModificacion)
    {
        return new Tramite(id, expedienteId, usuarioUltimoCambio, etiqueta, contenido, fechaCreacion, fechaUltimaModificacion);
    }

    public void Modificar(EtiquetaTramite nuevaEtiqueta, string nuevoContenido, Guid idUsuario)
    {
        Etiqueta = nuevaEtiqueta;
        Contenido = new ContenidoTramite(nuevoContenido);
        UsuarioUltimoCambio = idUsuario;
        FechaUltimaModificacion = DateTime.Now;
    }
}