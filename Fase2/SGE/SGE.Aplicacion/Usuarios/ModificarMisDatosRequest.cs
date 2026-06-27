namespace SGE.Aplicacion.Usuarios;

public record class ModificarMisDatosRequest(
    Guid IdUsuarioAutenticado,
    Guid IdUsuario,
    string? Nombre,
    string? CorreoElectronico,
    string? Contrasena);
