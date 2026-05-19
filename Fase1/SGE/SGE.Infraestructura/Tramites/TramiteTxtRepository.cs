using System.Collections.Generic;
using System.IO;
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
        string linea = $"{tramite.Id}|{tramite.ExpedienteId}|{tramite.UsuarioUltimoCambio}|{(int)tramite.Etiqueta}|{tramite.FechaCreacion}|{tramite.FechaUltimaModificacion}|{tramite.Contenido.Valor}{Environment.NewLine}";
        File.AppendAllText(_rutaArchivo, linea);
    }

    public void Modificar(Tramite tramite)
    {
        var lista = LeerTodosLosRegistros();
        int indice = -1;

        for (int i = 0; i < lista.Count; i++) {
            if (lista[i].Id == tramite.Id) {
                indice = i;
                break;
            }
        }

        if (indice < 0)
            throw new RepositorioException($"No se encontró el trámite con ID {tramite.Id} para modificar.");

        lista[indice] = tramite;
        GuardarTodos(lista);
    }

    public void Eliminar(Tramite tramite)
    {
        var lista = LeerTodosLosRegistros();
        int indice = -1;

        for (int i = 0; i < lista.Count; i++) {
            if (lista[i].Id == tramite.Id) {
                indice = i;
                break;
            }
        }

        if (indice < 0)
            throw new RepositorioException($"No se encontró el trámite con ID {tramite.Id} para eliminar.");

        lista.RemoveAt(indice);
        GuardarTodos(lista);
    }

    public Tramite? ObtenerPorId(Guid id)
    {
        foreach (var tramite in LeerTodosLosRegistros()) {
            if (tramite.Id == id)
                return tramite;
        }
        return null;
    }

    public IEnumerable<Tramite> ListarPorExpediente(Guid idExpediente)
    {
        var resultado = new List<Tramite>();

        if (!File.Exists(_rutaArchivo))
            return resultado;

        foreach (var tramite in LeerTodosLosRegistros()) {
            if (tramite.ExpedienteId == idExpediente)
                resultado.Add(tramite);
        }

        return resultado;
    }

    private List<Tramite> LeerTodosLosRegistros()
    {
        var tramites = new List<Tramite>();

        if (!File.Exists(_rutaArchivo))
            return tramites;

        string[] lineas = File.ReadAllLines(_rutaArchivo);

        foreach (string linea in lineas) {
            if (string.IsNullOrWhiteSpace(linea))
                continue;

            string[] partes = linea.Split('|');

            if (partes.Length != 7)
                throw new InvalidOperationException("El registro de tramite no tiene un formato valido.");

            Guid id = Guid.Parse(partes[0]);
            Guid expedienteId = Guid.Parse(partes[1]);
            Guid usuarioUltimoCambio = Guid.Parse(partes[2]);
            var etiqueta = (EtiquetaTramite)int.Parse(partes[3]);
            DateTime fechaCreacion = DateTime.Parse(partes[4]);
            DateTime fechaUltimaModificacion = DateTime.Parse(partes[5]);
            string contenido = partes[6];

            tramites.Add(Tramite.Reconstruir(
                id,
                expedienteId,
                usuarioUltimoCambio,
                etiqueta,
                new ContenidoTramite(contenido),
                fechaCreacion,
                fechaUltimaModificacion));
        }

        return tramites;
    }

    private void GuardarTodos(IEnumerable<Tramite> tramites)
    {
        var lineas = new List<string>();

        foreach (var t in tramites) {
            lineas.Add($"{t.Id}|{t.ExpedienteId}|{t.UsuarioUltimoCambio}|{(int)t.Etiqueta}|{t.FechaCreacion}|{t.FechaUltimaModificacion}|{t.Contenido.Valor}");
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