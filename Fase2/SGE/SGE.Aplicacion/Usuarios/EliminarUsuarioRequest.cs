namespace SGE.Aplicacion.Usuarios;

public record class EliminarUsuarioRequest(Guid IdUsuarioEjecutor, Guid IdUsuario);
