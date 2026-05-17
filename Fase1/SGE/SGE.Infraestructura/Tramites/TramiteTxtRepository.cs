using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SGE.Aplicacion.Tramites;
using SGE.Dominio.Tramites;
using SGE.Infraestructura.Comun;

namespace SGE.Infraestructura.Tramites;

public class TramiteTxtRepository : ITramiteRepository
{
    private readonly string _rutaArchivo;

    public TramiteTxtRepository(string? rutaArchivo = null)
    {
        _rutaArchivo = rutaArchivo ?? Path.Combine(AppContext.BaseDirectory, "tramites.txt");
        CrearDirectorioSiHaceFalta();
    }

    public void Agregar(Tramite tramite)
    {
        string linea = $"{tramite.Id}|{tramite.ExpedienteId}|{tramite.IdUsuario}|{(int)tramite.Etiqueta}|{tramite.FechaCreacion}|{tramite.FechaUltimaModificacion}|{tramite.Contenido.Valor}{Environment.NewLine}";
        File.AppendAllText(_rutaArchivo, linea);
    }

    public void Modificar(Tramite tramite)
    {
        var tramites = LeerTodosLosRegistros();
        int indice = tramites.FindIndex(t => t.Id == tramite.Id);

        if (indice < 0)
        {
            throw new RepositorioException($"No se encontró el trámite con ID {tramite.Id} para modificar.");
        }

        tramites[indice] = tramite;
        GuardarTodos(tramites);
    }

    public void Eliminar(Tramite tramite)
    {
        var tramites = LeerTodosLosRegistros();
        int indice = tramites.FindIndex(t => t.Id == tramite.Id);

        if (indice < 0)
        {
            throw new RepositorioException($"No se encontró el trámite con ID {tramite.Id} para eliminar.");
        }

        tramites.RemoveAt(indice);
        GuardarTodos(tramites);
    }

    public Tramite ObtenerPorId(Guid id)
    {
        var tramite = LeerTodosLosRegistros().FirstOrDefault(t => t.Id == id);

        if (tramite == null)
        {
            throw new RepositorioException($"No se encontró el trámite con ID {id}.");
        }

        return tramite;
    }


    public IEnumerable<Tramite> ListarPorExpediente()
    {
        var tramites = new List<Tramite>();

        if (!File.Exists(_rutaArchivo))
        {
            return tramites;
        }

        string[] lineas = File.ReadAllLines(_rutaArchivo);

        foreach (string linea in lineas)
        {
            if (string.IsNullOrWhiteSpace(linea))
            {
                continue;
            }

            string[] partes = linea.Split('|');

            if (partes.Length != 7)
            {
                throw new InvalidOperationException("El registro de tramite no tiene un formato valido.");
            }

            Guid id = Guid.Parse(partes[0]);
            Guid expedienteId = Guid.Parse(partes[1]);
            Guid idUsuario = Guid.Parse(partes[2]);
            var etiqueta = (EtiquetaTramite)Enum.Parse(typeof(EtiquetaTramite), partes[3]);
            DateTime fechaCreacion = DateTime.Parse(partes[4]);
            DateTime fechaUltimaModificacion = DateTime.Parse(partes[5]);
            string contenido = partes[6];

            tramites.Add(Tramite.Reconstruir(
                id,
                expedienteId,
                idUsuario,
                etiqueta,
                new ContenidoTramite(contenido),
                fechaCreacion,
                fechaUltimaModificacion));
        }

        return tramites;
    }

    private void GuardarTodos(IEnumerable<Tramite> tramites)
    {
        var lineas = tramites.Select(t =>
            $"{t.Id}|{t.ExpedienteId}|{t.IdUsuario}|{(int)t.Etiqueta}|{t.FechaCreacion}|{t.FechaUltimaModificacion}|{t.Contenido.Valor}");

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