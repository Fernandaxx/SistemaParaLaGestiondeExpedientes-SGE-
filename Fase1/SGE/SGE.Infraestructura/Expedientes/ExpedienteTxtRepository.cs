using System.Globalization;
using System.Text;
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
    }

    public void Agregar(Expediente expediente)
    {
        var expedientes = ListarTodos();
        expedientes.Add(expediente);
        GuardarTodos(expedientes);
    }

    public void Modificar(Expediente expediente)
    {
        var expedientes = ListarTodos();
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
        var expedientes = ListarTodos();
        int index = expedientes.FindIndex(e => e.Id == expediente.Id);

        if (index < 0)
        {
            throw new RepositorioException($"No se encontró el expediente con ID {expediente.Id} para eliminar.");
        }

        expedientes.RemoveAt(index);
        GuardarTodos(expedientes);
    }

    public Expediente? ObtenerPorId(Guid id)
    {
        return ListarTodos().FirstOrDefault(e => e.Id == id);
    }

    public List<Expediente> ListarTodos()
    {
        var lista = new List<Expediente>();

        if (!File.Exists(_rutaArchivo))
        {
            return lista;
        }

        string[] lineas = File.ReadAllLines(_rutaArchivo, Encoding.UTF8);

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
            string valorCaratula = Decodificar(campos[1]);
            Guid idUsuario = Guid.Parse(campos[2]);
            EstadoExpediente estado = Enum.Parse<EstadoExpediente>(campos[3]);
            DateTime fechaCreacion = DateTime.ParseExact(campos[4], "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            DateTime fechaModificacion = DateTime.ParseExact(campos[5], "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

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
        CrearDirectorioSiHaceFalta();

        var lineas = expedientes.Select(Serializar).ToArray();
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

    private static string Serializar(Expediente expediente)
    {
        return string.Join('|',
            expediente.Id,
            Codificar(expediente.Caratula.Valor),
            expediente.IdUsuario,
            expediente.Estado,
            expediente.FechaCreacion.ToString("O", CultureInfo.InvariantCulture),
            expediente.FechaModificacion.ToString("O", CultureInfo.InvariantCulture));
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