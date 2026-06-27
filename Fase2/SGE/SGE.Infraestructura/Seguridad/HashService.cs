using System.Security.Cryptography;
using System.Text;
using SGE.Aplicacion.Autorizacion;

namespace SGE.Infraestructura.Seguridad;

public class HashService : IHashService
{
    public string GenerarHash(string valor)
    {
        var bytes = Encoding.UTF8.GetBytes(valor);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
