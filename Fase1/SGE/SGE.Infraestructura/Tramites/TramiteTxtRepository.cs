using System.Globalization;
using System.Text;
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
    }

    public void Agregar(Tramite tramite)
    {
        var tramites = ListarTodos();
        tramites.Add(tramite);
        GuardarTodos(tramites);
    }

    public void Modificar(Tramite tramite)
    {
        var tramites = ListarTodos();
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
        var tramites = ListarTodos();
        int indice = tramites.FindIndex(t => t.Id == tramite.Id);

        if (indice < 0)
        {
            throw new RepositorioException($"No se encontró el trámite con ID {tramite.Id} para eliminar.");
        }

        tramites.RemoveAt(indice);
        GuardarTodos(tramites);
    }

    public Tramite? ObtenerPorId(Guid id)
    {
        return ListarTodos().FirstOrDefault(t => t.Id == id);
    }



    public List<Tramite> ListarTodos()
    {
        if (!File.Exists(_rutaArchivo))
        {
            return [];
        }

        var tramites = new List<Tramite>();

        foreach (string linea in File.ReadAllLines(_rutaArchivo, Encoding.UTF8))
        {
            if (string.IsNullOrWhiteSpace(linea))
            {
                continue;
            }

            tramites.Add(Deserializar(linea));
        }

        return tramites;
    }

    private void GuardarTodos(IEnumerable<Tramite> tramites)
    {
        CrearDirectorioSiHaceFalta();

        var lineas = tramites.Select(Serializar).ToArray();
        File.WriteAllLines(_rutaArchivo, lineas, Encoding.UTF8);
    }

    private void CrearDirectorioSiHaceFalta()
    {
        string? directorio = Path.GetDirectoryName(_rutaArchivo);

        if (!string.IsNullOrWhiteSpace(directorio))
        {
            Directory.CreateDirectory(directorio);
        }
    }

    private static string Serializar(Tramite tramite)
    {
        return string.Join('|',
            tramite.Id,
            tramite.ExpedienteId,
            tramite.IdUsuario,
            (int)tramite.Etiqueta,
            tramite.FechaCreacion.ToString("O", CultureInfo.InvariantCulture),
            tramite.FechaUltimaModificacion.ToString("O", CultureInfo.InvariantCulture),
            Codificar(tramite.Contenido.Valor));
    }

    private static Tramite Deserializar(string linea)
    {
        string[] partes = linea.Split('|');

        if (partes.Length != 7)
        {
            throw new InvalidOperationException("El registro de tramite no tiene un formato valido.");
        }

        Guid id = Guid.Parse(partes[0]);
        Guid expedienteId = Guid.Parse(partes[1]);
        Guid idUsuario = Guid.Parse(partes[2]);
        var etiqueta = (EtiquetaTramite)Enum.Parse(typeof(EtiquetaTramite), partes[3]);
        DateTime fechaCreacion = DateTime.ParseExact(partes[4], "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        DateTime fechaUltimaModificacion = DateTime.ParseExact(partes[5], "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        string contenido = Decodificar(partes[6]);

        return Tramite.Reconstruir(id, expedienteId, idUsuario, etiqueta, new ContenidoTramite(contenido), fechaCreacion, fechaUltimaModificacion);
    }

    private static string Codificar(string valor)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(valor));
    }

    private static string Decodificar(string valor)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(valor));
    }
}
