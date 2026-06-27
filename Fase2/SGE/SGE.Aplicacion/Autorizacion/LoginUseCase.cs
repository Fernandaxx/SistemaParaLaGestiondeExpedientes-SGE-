using SGE.Aplicacion.Usuarios;
using SGE.Dominio.Comun;

namespace SGE.Aplicacion.Autorizacion;

public class LoginUseCase(IUsuarioRepository _repository, IHashService _hashService, ITokenService _tokenService)
{
    public LoginResponse Ejecutar(LoginRequest request)
    {
        var correoElectronico = request.CorreoElectronico?.Trim() ?? string.Empty;
        var contrasena = ValidarContrasena(request.Contrasena);

        var usuario = _repository.ObtenerPorCorreoElectronico(correoElectronico);
        if (usuario is null)
            throw new AutorizacionException("Credenciales inválidas.");

        var contrasenaHash = _hashService.GenerarHash(contrasena);
        if (usuario.ContrasenaHash != contrasenaHash)
            throw new AutorizacionException("Credenciales inválidas.");

        var token = _tokenService.GenerarToken(usuario);
        return new LoginResponse(usuario.Id, token);
    }

    private static string ValidarContrasena(string contrasena)
    {
        if (string.IsNullOrWhiteSpace(contrasena)) throw new DominioException("La contraseña es obligatoria.");

        return contrasena;
    }
}
