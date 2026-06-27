using SGE.Dominio.Usuarios;

namespace SGE.Aplicacion.Usuarios;

public record class UsuarioDTO(
    Guid Id,
    string Nombre,
    string CorreoElectronico,
    bool EsAdministrador,
    IEnumerable<Permiso> Permisos);
