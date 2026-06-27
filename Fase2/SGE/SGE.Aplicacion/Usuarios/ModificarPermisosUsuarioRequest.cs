using SGE.Dominio.Usuarios;

namespace SGE.Aplicacion.Usuarios;

public record class ModificarPermisosUsuarioRequest(Guid IdUsuarioEjecutor, Guid IdUsuario, IEnumerable<Permiso> Permisos);
