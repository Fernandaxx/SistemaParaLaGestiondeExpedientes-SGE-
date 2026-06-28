using SGE.Aplicacion.Autorizacion;
using SGE.Dominio.Usuarios;

namespace SGE.Aplicacion.Usuarios;

public class ListarUsuariosUseCase(IUsuarioRepository _repository, IAutorizacionService _autorizacionService)
{
    public ListarUsuariosResponse Ejecutar(ListarUsuariosRequest request)
    {
        if (!_autorizacionService.EsAdministrador(request.IdUsuarioEjecutor))
            throw new AutorizacionException("Usuario no autorizado para gestionar usuarios.");

        var usuarios = _repository.ListarTodos()
            .Select(MapearUsuario)
            .ToList();

        return new ListarUsuariosResponse(usuarios);
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