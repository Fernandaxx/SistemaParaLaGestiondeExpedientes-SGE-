using SGE.Aplicacion.Autorizacion;
using SGE.Dominio.Usuarios;

namespace SGE.Aplicacion.Usuarios;

public class ListarUsuariosUseCase(IUsuarioRepository _repository)
{
    public ListarUsuariosResponse Ejecutar(ListarUsuariosRequest request)
    {
        VerificarAdministrador(request.IdUsuarioEjecutor);

        var usuarios = _repository.ListarTodos()
            .Select(MapearUsuario)
            .ToList();

        return new ListarUsuariosResponse(usuarios);
    }

    private void VerificarAdministrador(Guid idUsuarioEjecutor)
    {
        var usuarioEjecutor = _repository.ObtenerPorId(idUsuarioEjecutor);
        if (usuarioEjecutor is null || !usuarioEjecutor.EsAdministrador)
            throw new AutorizacionException("Usuario no autorizado para gestionar usuarios.");
    }

    private static UsuarioDTO MapearUsuario(Usuario usuario)
    {
        return new UsuarioDTO(
            usuario.Id,
            usuario.Nombre,
            usuario.CorreoElectronico,
            usuario.EsAdministrador,
            usuario.Permisos.ToList());
    }
}
