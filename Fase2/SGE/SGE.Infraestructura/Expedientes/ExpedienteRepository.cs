using Microsoft.EntityFrameworkCore;
using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Expedientes;
using SGE.Infraestructura.Comun;
using SGE.Infraestructura.Datos;

namespace SGE.Infraestructura.Expedientes;

public class ExpedienteRepository(SgeContext _context) : IExpedienteRepository
{
    public void Agregar(Expediente expediente)
    {
        _context.Expedientes.Add(expediente);
    }

    public void Modificar(Expediente expediente)
    {
        if (!Existe(expediente.Id))
            throw new RepositorioException($"No se encontró el expediente con ID {expediente.Id} para modificar.");

        var expedienteLocal = _context.Expedientes.Local.FirstOrDefault(e => e.Id == expediente.Id);
        if (expedienteLocal is not null && !ReferenceEquals(expedienteLocal, expediente)) {
            _context.Entry(expedienteLocal).CurrentValues.SetValues(expediente);
            _context.Entry(expedienteLocal).State = EntityState.Modified;
            return;
        }

        _context.Expedientes.Update(expediente);
    }

    public void Eliminar(Expediente expediente)
    {
        var expedienteLocal = _context.Expedientes.Local.FirstOrDefault(e => e.Id == expediente.Id);
        if (expedienteLocal is not null) {
            _context.Expedientes.Remove(expedienteLocal);
            return;
        }

        if (!_context.Expedientes.Any(e => e.Id == expediente.Id))
            throw new RepositorioException($"No se encontró el expediente con ID {expediente.Id} para eliminar.");

        _context.Expedientes.Attach(expediente);
        _context.Expedientes.Remove(expediente);
    }

    public Expediente? ObtenerPorId(Guid id)
    {
        var expediente = _context.Expedientes.Find(id);
        if (expediente is null) return null;

        return _context.Entry(expediente).State == EntityState.Deleted
            ? null
            : expediente;
    }

    public IEnumerable<Expediente> ListarTodos()
    {
        var expedientes = _context.Expedientes.ToList();
        return AplicarCambiosLocales(expedientes);
    }

    private bool Existe(Guid id)
    {
        var expedienteLocal = _context.Expedientes.Local.FirstOrDefault(e => e.Id == id);
        if (expedienteLocal is not null)
            return _context.Entry(expedienteLocal).State != EntityState.Deleted;

        return _context.Expedientes.Any(e => e.Id == id);
    }

    private IReadOnlyCollection<Expediente> AplicarCambiosLocales(IEnumerable<Expediente> expedientes)
    {
        var resultado = expedientes.ToDictionary(e => e.Id);

        foreach (var entry in _context.ChangeTracker.Entries<Expediente>()) {
            if (entry.State == EntityState.Detached) continue;

            if (entry.State == EntityState.Deleted) {
                resultado.Remove(entry.Entity.Id);
            } else {
                resultado[entry.Entity.Id] = entry.Entity;
            }
        }

        return resultado.Values.ToList();
    }
}
