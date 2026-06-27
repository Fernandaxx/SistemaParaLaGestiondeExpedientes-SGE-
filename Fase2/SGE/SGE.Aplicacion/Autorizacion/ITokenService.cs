using SGE.Dominio.Usuarios;

namespace SGE.Aplicacion.Autorizacion;

public interface ITokenService
{
    string GenerarToken(Usuario usuario);
}
