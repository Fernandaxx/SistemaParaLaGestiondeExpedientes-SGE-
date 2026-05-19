using System.Collections.Generic;
using System.IO;
using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Expedientes;
using SGE.Infraestructura.Comun;

namespace SGE.Infraestructura.Expedientes;

public class ExpedienteTxtRepository : IExpedienteRepository
{
    private readonly string _rutaArchivo;

    public ExpedienteTxtRepository(string? rutaArchivo = null)
    {
        _rutaArchivo = rutaArchivo ?? Path.Combine(AppContext.BaseDirectory, "expedientes.txt");
        CrearDirectorioSiHaceFalta();
    }

    public void Agregar(Expediente expediente)
    {
        string linea = $"{expediente.Id}|{expediente.Caratula.Valor}|{expediente.UsuarioUltimoCambio}|{expediente.Estado}|{expediente.FechaCreacion}|{expediente.FechaModificacion}{Environment.NewLine}";
        File.AppendAllText(_rutaArchivo, linea);
    }

    public void Modificar(Expediente expediente)
    {
        var expedientes = ListarTodos();
        var lista = new List<Expediente>(expedientes);
        int index = -1;

        for (int i = 0; i < lista.Count; i++) {
            if (lista[i].Id == expediente.Id) {
                index = i;
                break;
            }
        }

        if (index < 0)
            throw new RepositorioException($"No se encontró el expediente con ID {expediente.Id} para modificar.");

        lista[index] = expediente;
        GuardarTodos(lista);
    }

    public void Eliminar(Expediente expediente)
    {
        var expedientes = ListarTodos();
        var lista = new List<Expediente>(expedientes);
        int index = -1;

        for (int i = 0; i < lista.Count; i++) {
            if (lista[i].Id == expediente.Id) {
                index = i;
                break;
            }
        }

        if (index < 0)
            throw new RepositorioException($"No se encontró el expediente con ID {expediente.Id} para eliminar.");

        lista.RemoveAt(index);
        GuardarTodos(lista);
    }

    public Expediente? ObtenerPorId(Guid id)
    {
        foreach (var expediente in ListarTodos()) {
            if (expediente.Id == id)
                return expediente;
        }
        return null;
    }

    public IEnumerable<Expediente> ListarTodos()
    {
        var lista = new List<Expediente>();

        if (!File.Exists(_rutaArchivo))
            return lista;

        string[] lineas = File.ReadAllLines(_rutaArchivo);

        foreach (string linea in lineas) {
            if (string.IsNullOrWhiteSpace(linea))
                continue;

            string[] campos = linea.Split('|');

            if (campos.Length != 6)
                throw new InvalidOperationException("El registro de expediente no tiene un formato valido.");

            Guid id = Guid.Parse(campos[0]);
            string valorCaratula = campos[1];
            Guid usuarioUltimoCambio = Guid.Parse(campos[2]);
            EstadoExpediente estado = Enum.Parse<EstadoExpediente>(campos[3]);
            DateTime fechaCreacion = DateTime.Parse(campos[4]);
            DateTime fechaModificacion = DateTime.Parse(campos[5]);

            lista.Add(Expediente.Reconstruir(id, new Caratula(valorCaratula), usuarioUltimoCambio, estado, fechaCreacion, fechaModificacion));
        }

        return lista;
    }

    private void GuardarTodos(IEnumerable<Expediente> expedientes)
    {
        var lineas = new List<string>();

        foreach (var e in expedientes) {
            lineas.Add($"{e.Id}|{e.Caratula.Valor}|{e.UsuarioUltimoCambio}|{e.Estado}|{e.FechaCreacion}|{e.FechaModificacion}");
        }
        File.WriteAllLines(_rutaArchivo, lineas);
    }

    private void CrearDirectorioSiHaceFalta()
    {
        string? directorio = Path.GetDirectoryName(_rutaArchivo);
        if (!string.IsNullOrWhiteSpace(directorio)) {
            Directory.CreateDirectory(directorio);
        }
    }
}