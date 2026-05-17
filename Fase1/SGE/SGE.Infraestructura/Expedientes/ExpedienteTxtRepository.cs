using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        string linea = $"{expediente.Id}|{expediente.Caratula.Valor}|{expediente.IdUsuario}|{expediente.Estado}|{expediente.FechaCreacion}|{expediente.FechaModificacion}{Environment.NewLine}";
        File.AppendAllText(_rutaArchivo, linea);
    }

    public void Modificar(Expediente expediente)
    {
        var expedientes = ListarTodos().ToList();
        int index = expedientes.FindIndex(e => e.Id == expediente.Id);

        if (index < 0)
        {
            throw new RepositorioException($"No se encontró el expediente con ID {expediente.Id} para modificar.");
        }

        expedientes[index] = expediente;
        GuardarTodos(expedientes);
    }

    public void Eliminar(Expediente expediente)
    {
        var expedientes = ListarTodos().ToList();
        int index = expedientes.FindIndex(e => e.Id == expediente.Id);

        if (index < 0)
        {
            throw new RepositorioException($"No se encontró el expediente con ID {expediente.Id} para eliminar.");
        }

        expedientes.RemoveAt(index);
        GuardarTodos(expedientes);
    }

    public Expediente ObtenerPorId(Guid id)
    {
        var expediente = ListarTodos().FirstOrDefault(e => e.Id == id);

        if (expediente == null)
        {
            throw new RepositorioException($"No se encontró el expediente con ID {id}.");
        }

        return expediente;
    }


    public IEnumerable<Expediente> ListarTodos()
    {
        var lista = new List<Expediente>();

        if (!File.Exists(_rutaArchivo))
        {
            return lista;
        }

        string[] lineas = File.ReadAllLines(_rutaArchivo);

        foreach (string linea in lineas)
        {
            if (string.IsNullOrWhiteSpace(linea))
            {
                continue;
            }

            string[] campos = linea.Split('|');

            if (campos.Length != 6)
            {
                throw new InvalidOperationException("El registro de expediente no tiene un formato valido.");
            }

            Guid id = Guid.Parse(campos[0]);
            string valorCaratula = campos[1];
            Guid idUsuario = Guid.Parse(campos[2]);
            EstadoExpediente estado = Enum.Parse<EstadoExpediente>(campos[3]);
            DateTime fechaCreacion = DateTime.Parse(campos[4]);
            DateTime fechaModificacion = DateTime.Parse(campos[5]);

            lista.Add(
                Expediente.Reconstruir(
                    id,
                    new Caratula(valorCaratula),
                    idUsuario,
                    estado,
                    fechaCreacion,
                    fechaModificacion));
        }

        return lista;
    }

    private void GuardarTodos(IEnumerable<Expediente> expedientes)
    {
        var lineas = expedientes.Select(e =>
            $"{e.Id}|{e.Caratula.Valor}|{e.IdUsuario}|{e.Estado}|{e.FechaCreacion}|{e.FechaModificacion}");

        File.WriteAllLines(_rutaArchivo, lineas);
    }

    private void CrearDirectorioSiHaceFalta()
    {
        string? directorio = Path.GetDirectoryName(_rutaArchivo);

        if (!string.IsNullOrWhiteSpace(directorio))
        {
            Directory.CreateDirectory(directorio);
        }
    }
}