using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Comun;

namespace SGE.Aplicacion.Usuarios;

public class ModificarMisDatosUseCase(IUsuarioRepository _repository, IHashService _hashService, IUnidadDeTrabajo _unidadDeTrabajo)
{
    public ModificarMisDatosResponse Ejecutar(ModificarMisDatosRequest request)
    {
        if (request.IdUsuarioAutenticado != request.IdUsuario)
            throw new AutorizacionException("Un usuario no puede modificar los datos de otro usuario.");

        var usuario = _repository.ObtenerPorId(request.IdUsuario);
        if (usuario is null)
            throw new EntidadNoEncontradaException("No se encontró el usuario solicitado.");

        var nuevoNombre = request.Nombre ?? usuario.Nombre;
        var nuevoCorreoElectronico = request.CorreoElectronico?.Trim() ?? usuario.CorreoElectronico;

        if (!string.Equals(nuevoCorreoElectronico, usuario.CorreoElectronico, StringComparison.OrdinalIgnoreCase)) {
            var usuarioConCorreo = _repository.ObtenerPorCorreoElectronico(nuevoCorreoElectronico);
            if (usuarioConCorreo is not null && usuarioConCorreo.Id != usuario.Id)
                throw new DominioException("Ya existe un usuario registrado con ese correo electrónico.");
        }

        usuario.ModificarDatos(nuevoNombre, nuevoCorreoElectronico);

        if (request.Contrasena is not null) {
            usuario.CambiarContrasenaHash(_hashService.GenerarHash(ValidarContrasena(request.Contrasena)));
        }

        _repository.Modificar(usuario);
        _unidadDeTrabajo.Guardar();

        return new ModificarMisDatosResponse();
    }

    private static string ValidarContrasena(string contrasena)
    {
        if (string.IsNullOrWhiteSpace(contrasena)) throw new DominioException("La contraseña es obligatoria.");

        return contrasena;
    }
}
