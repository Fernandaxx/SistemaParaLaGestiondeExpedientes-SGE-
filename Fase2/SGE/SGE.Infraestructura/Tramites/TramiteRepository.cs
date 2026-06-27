using Microsoft.EntityFrameworkCore;
using SGE.Aplicacion.Tramites;
using SGE.Dominio.Tramites;
using SGE.Infraestructura.Comun;
using SGE.Infraestructura.Datos;

namespace SGE.Infraestructura.Tramites;

public class TramiteRepository(SgeContext _context) : ITramiteRepository
{
    public void Agregar(Tramite tramite)
    {
        _context.Tramites.Add(tramite);
    }

    public void Modificar(Tramite tramite)
    {
        if (!Existe(tramite.Id))
            throw new RepositorioException($"No se encontró el trámite con ID {tramite.Id} para modificar.");

        var tramiteLocal = _context.Tramites.Local.FirstOrDefault(t => t.Id == tramite.Id);
        if (tramiteLocal is not null && !ReferenceEquals(tramiteLocal, tramite)) {
            _context.Entry(tramiteLocal).CurrentValues.SetValues(tramite);
            _context.Entry(tramiteLocal).State = EntityState.Modified;
            return;
        }

        _context.Tramites.Update(tramite);
    }

    public void Eliminar(Tramite tramite)
    {
        var tramiteLocal = _context.Tramites.Local.FirstOrDefault(t => t.Id == tramite.Id);
        if (tramiteLocal is not null) {
            _context.Tramites.Remove(tramiteLocal);
            return;
        }

        if (!_context.Tramites.Any(t => t.Id == tramite.Id))
            throw new RepositorioException($"No se encontró el trámite con ID {tramite.Id} para eliminar.");

        _context.Tramites.Attach(tramite);
        _context.Tramites.Remove(tramite);
    }

    public Tramite? ObtenerPorId(Guid id)
    {
        var tramite = _context.Tramites.Find(id);
        if (tramite is null) return null;

        return _context.Entry(tramite).State == EntityState.Deleted
            ? null
            : tramite;
    }

    public IEnumerable<Tramite> ListarPorExpediente(Guid idExpediente)
    {
        var tramites = _context.Tramites
            .Where(t => t.ExpedienteId == idExpediente)
            .ToList();

        return AplicarCambiosLocales(tramites, idExpediente);
    }

    private bool Existe(Guid id)
    {
        var tramiteLocal = _context.Tramites.Local.FirstOrDefault(t => t.Id == id);
        if (tramiteLocal is not null)
            return _context.Entry(tramiteLocal).State != EntityState.Deleted;

        return _context.Tramites.Any(t => t.Id == id);
    }

    private IReadOnlyCollection<Tramite> AplicarCambiosLocales(IEnumerable<Tramite> tramites, Guid idExpediente)
    {
        var resultado = tramites.ToDictionary(t => t.Id);

        foreach (var entry in _context.ChangeTracker.Entries<Tramite>()) {
            if (entry.State == EntityState.Detached) continue;

            if (entry.State == EntityState.Deleted) {
                resultado.Remove(entry.Entity.Id);
                continue;
            }

            if (entry.Entity.ExpedienteId == idExpediente) {
                resultado[entry.Entity.Id] = entry.Entity;
            }
        }

        return resultado.Values
            .OrderBy(t => t.FechaCreacion)
            .ToList();
    }
}
