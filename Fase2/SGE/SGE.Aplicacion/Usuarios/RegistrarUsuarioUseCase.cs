using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Comun;
using SGE.Dominio.Usuarios;

namespace SGE.Aplicacion.Usuarios;

public class RegistrarUsuarioUseCase(IUsuarioRepository _repository, IHashService _hashService, IUnidadDeTrabajo _unidadDeTrabajo)
{
    public RegistrarUsuarioResponse Ejecutar(RegistrarUsuarioRequest request)
    {
        var correoElectronico = request.CorreoElectronico?.Trim() ?? string.Empty;

        if (_repository.ObtenerPorCorreoElectronico(correoElectronico) is not null)
            throw new DominioException("Ya existe un usuario registrado con ese correo electrónico.");

        var contrasenaHash = _hashService.GenerarHash(ValidarContrasena(request.Contrasena));
        var usuario = new Usuario(request.Nombre, correoElectronico, contrasenaHash);

        _repository.Agregar(usuario);
        _unidadDeTrabajo.Guardar();

        return new RegistrarUsuarioResponse(usuario.Id);
    }

    private static string ValidarContrasena(string contrasena)
    {
        if (string.IsNullOrWhiteSpace(contrasena)) throw new DominioException("La contraseña es obligatoria.");

        return contrasena;
    }
}
