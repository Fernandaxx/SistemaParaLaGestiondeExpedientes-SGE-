using SGE.Dominio.Comun;
using SGE.Dominio.Tramites;

namespace SGE.Dominio.Expedientes;

public class Expediente
{
    public Guid Id { get; private set; }
    public Caratula Caratula { get; private set; }
    public Guid UsuarioUltimoCambio { get; private set; }
    public EstadoExpediente Estado { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaModificacion { get; private set; }

    protected Expediente()
    {
        Caratula = null!;
    }

    public Expediente(Caratula caratula, Guid usuarioUltimoCambio)
    {
        if (caratula == null) throw new DominioException("La carátula es obligatoria.");

        if (usuarioUltimoCambio == Guid.Empty) throw new DominioException("Debe especificarse un usuario válido.");

        Id = Guid.NewGuid();
        Caratula = caratula;
        UsuarioUltimoCambio = usuarioUltimoCambio;
        Estado = EstadoExpediente.RecienIniciado;
        DateTime ahora = DateTime.Now;
        FechaCreacion = ahora;
        FechaModificacion = ahora;
    }

    private Expediente(Guid id, Caratula caratula, Guid usuarioUltimoCambio, EstadoExpediente estado, DateTime fechaCreacion, DateTime fechaModificacion)
    {
        if (id == Guid.Empty) throw new DominioException("ID inválido.");

        if (caratula == null) throw new DominioException("Carátula obligatoria.");

        if (usuarioUltimoCambio == Guid.Empty) throw new DominioException("Usuario inválido.");

        if (!Enum.IsDefined(typeof(EstadoExpediente), estado)) throw new DominioException("Estado inválido.");

        if (fechaCreacion == default) throw new DominioException("Fecha de creación inválida.");

        if (fechaModificacion == default) throw new DominioException("Fecha de modificación inválida.");

        if (fechaModificacion < fechaCreacion) throw new DominioException("La fecha de modificación no puede ser anterior a la fecha de creación.");

        Id = id;
        Caratula = caratula;
        UsuarioUltimoCambio = usuarioUltimoCambio;
        Estado = estado;
        FechaCreacion = fechaCreacion;
        FechaModificacion = fechaModificacion;
    }

    public static Expediente Reconstruir(Guid id, Caratula caratula, Guid usuarioUltimoCambio, EstadoExpediente estado, DateTime fechaCreacion, DateTime fechaModificacion)
    {
        return new Expediente(id, caratula, usuarioUltimoCambio, estado, fechaCreacion, fechaModificacion);
    }

    public void ModificarCaratula(Caratula nuevaCaratula, Guid idUsuarioModificador)
    {
        if (nuevaCaratula == null) throw new DominioException("La nueva carátula no puede ser nula.");

        if (idUsuarioModificador == Guid.Empty) throw new DominioException("Usuario inválido.");

        Caratula = nuevaCaratula;
        UsuarioUltimoCambio = idUsuarioModificador;
        FechaModificacion = DateTime.Now;
    }

    public bool ActualizarEstado(EtiquetaTramite? ultimaEtiqueta, Guid idUsuario)
    {
        if (idUsuario == Guid.Empty) throw new DominioException("Usuario inválido.");

        EstadoExpediente nuevoEstado = ultimaEtiqueta switch
        {
            EtiquetaTramite.Resolucion => EstadoExpediente.ConResolucion,
            EtiquetaTramite.PaseAEstudio => EstadoExpediente.ParaResolver,
            EtiquetaTramite.PaseAlArchivo => EstadoExpediente.Finalizado,
            null => EstadoExpediente.RecienIniciado,
            _ => Estado
        };

        if (Estado == nuevoEstado) return false;

        Estado = nuevoEstado;
        UsuarioUltimoCambio = idUsuario;
        FechaModificacion = DateTime.Now;
        return true;
    }

    public void CambiarEstado(EstadoExpediente nuevoEstado, Guid idUsuarioModificador)
    {
        if (idUsuarioModificador == Guid.Empty) throw new DominioException("Usuario inválido.");
        
        if (Estado == nuevoEstado) throw new DominioException("El expediente ya está en ese estado.");

        Estado = nuevoEstado;
        UsuarioUltimoCambio = idUsuarioModificador;
        FechaModificacion = DateTime.Now;
    }
}
