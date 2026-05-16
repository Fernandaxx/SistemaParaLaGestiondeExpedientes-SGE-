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
    }
}