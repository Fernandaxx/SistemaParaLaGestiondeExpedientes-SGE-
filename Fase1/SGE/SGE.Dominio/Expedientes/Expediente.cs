using SGE.Dominio.Comun;
using SGE.Dominio.Tramites;

namespace SGE.Dominio.Expedientes;

public class Expediente
{

    public Guid Id { get; private set; }
    public Caratula Caratula { get; private set; }
    public Guid IdUsuario { get; private set; }
    public EstadoExpediente Estado { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaModificacion { get; private set; }

    //constructor
    public Expediente(Caratula caratula, Guid idUsuario)
    {
        if (caratula == null)
        {
            throw new DominioException("La carátula es obligatoria para dar de alta un expediente.");
        }

        if (idUsuario == Guid.Empty)
        {
            throw new DominioException("Debe especificarse un usuario válido que da de alta el expediente.");
        }

        Id = Guid.NewGuid();
        Caratula = caratula;
        IdUsuario = idUsuario;
        Estado = EstadoExpediente.RecienIniciado;
        DateTime ahora = DateTime.Now;
        FechaCreacion = ahora;
        FechaModificacion = ahora;
    }

    private Expediente(Guid id, Caratula caratula, Guid idUsuario, EstadoExpediente estado, DateTime fechaCreacion, DateTime fechaModificacion)
    {
        if (id == Guid.Empty)
        {
            throw new DominioException("Debe especificarse un identificador válido para reconstruir el expediente.");
        }

        if (caratula == null)
        {
            throw new DominioException("La carátula es obligatoria para reconstruir un expediente.");
        }

        if (idUsuario == Guid.Empty)
        {
            throw new DominioException("Debe especificarse un usuario válido para reconstruir el expediente.");
        }

        if (!Enum.IsDefined(typeof(EstadoExpediente), estado))
        {
            throw new DominioException("El estado del expediente no es válido para su reconstrucción.");
        }

        if (fechaCreacion == default)
        {
            throw new DominioException("La fecha de creación es obligatoria para reconstruir el expediente.");
        }

        if (fechaModificacion == default)
        {
            throw new DominioException("La fecha de modificación es obligatoria para reconstruir el expediente.");
        }

        if (fechaModificacion < fechaCreacion)
        {
            throw new DominioException("La fecha de modificación no puede ser anterior a la fecha de creación.");
        }

        Id = id;
        Caratula = caratula;
        IdUsuario = idUsuario;
        Estado = estado;
        FechaCreacion = fechaCreacion;
        FechaModificacion = fechaModificacion;
    }
    //factory method (reconstrucción)
    public static Expediente Reconstruir(Guid id, Caratula caratula, Guid idUsuario, EstadoExpediente estado, DateTime fechaCreacion, DateTime fechaModificacion)
    {
        return new Expediente(id, caratula, idUsuario, estado, fechaCreacion, fechaModificacion);
    }


    //Logica de negocio: A
    public void ModificarCaratula(Caratula nuevaCaratula, Guid idUsuarioModificador)
    {
        if (nuevaCaratula == null)
        {
            throw new DominioException("La nueva carátula no puede ser nula.");
        }

        if (idUsuarioModificador == Guid.Empty)
        {
            throw new DominioException("Se requiere un usuario válido para registrar la modificación.");
        }

        Caratula = nuevaCaratula;
        IdUsuario = idUsuarioModificador;
        FechaModificacion = DateTime.Now;

        if (FechaModificacion < FechaCreacion)
        {
            throw new DominioException("Error de consistencia temporal en la actualización de la entidad.");
        }
    }

    //Logica de negocio: B (retorna true si hubo cambio de estado)
    public bool ActualizarEstado(EtiquetaTramite? ultimaEtiqueta, Guid idUsuario)
    {
        if (idUsuario == Guid.Empty)
        {
            throw new DominioException("Se requiere un usuario válido para registrar la actualización automática del estado.");
        }

        EstadoExpediente nuevoEstado = ultimaEtiqueta switch
        {
            EtiquetaTramite.Resolucion => EstadoExpediente.ConResolucion,
            EtiquetaTramite.PaseAEstudio => EstadoExpediente.ParaResolver,
            EtiquetaTramite.PaseAlArchivo => EstadoExpediente.Finalizado,
            null => EstadoExpediente.RecienIniciado,
            _ => Estado
        };

        if (Estado == nuevoEstado)
        {
            return false;
        }

        Estado = nuevoEstado;
        IdUsuario = idUsuario;
        FechaModificacion = DateTime.Now;

        if (FechaModificacion < FechaCreacion)
        {
            throw new DominioException("Error de consistencia temporal en la actualización de la entidad.");
        }

        return true;
    }

    //Logica de negocio: C

    public void CambiarEstado(EstadoExpediente nuevoEstado, Guid idUsuarioModificador)
    {
        if (idUsuarioModificador == Guid.Empty)
        {
            throw new DominioException("Se requiere un usuario válido para registrar el cambio de estado.");
        }

        if (Estado == nuevoEstado)
        {
            throw new DominioException("El expediente ya se encuentra en el estado especificado.");
        }

        Estado = nuevoEstado;
        IdUsuario = idUsuarioModificador;
        FechaModificacion = DateTime.Now;

        if (FechaModificacion < FechaCreacion)
        {
            throw new DominioException("Error de consistencia temporal en la actualización de la entidad.");
        }
    }




}