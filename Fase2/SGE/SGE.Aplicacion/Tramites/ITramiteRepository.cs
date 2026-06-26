using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Tramites;

public interface ITramiteRepository
{
    void Agregar(Tramite tramite);
    void Modificar(Tramite tramite);
    void Eliminar(Tramite tramite);

    Tramite? ObtenerPorId(Guid id);

    IEnumerable<Tramite> ListarPorExpediente(Guid idExpediente);
}