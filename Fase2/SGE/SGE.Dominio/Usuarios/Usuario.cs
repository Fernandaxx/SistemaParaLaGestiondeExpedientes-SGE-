using SGE.Dominio.Comun;

namespace SGE.Dominio.Usuarios;

public class Usuario
{
    private readonly List<Permiso> _permisos = [];

    public Guid Id { get; private set; }
    public string Nombre { get; private set; }
    public string CorreoElectronico { get; private set; }
    public string ContrasenaHash { get; private set; }
    public bool EsAdministrador { get; private set; }
    public IReadOnlyCollection<Permiso> Permisos => _permisos.AsReadOnly();

    public Usuario(string nombre, string correoElectronico, string contrasenaHash, bool esAdministrador = false)
    {
        Id = Guid.NewGuid();
        Nombre = ValidarNombre(nombre);
        CorreoElectronico = ValidarCorreoElectronico(correoElectronico);
        ContrasenaHash = ValidarContrasenaHash(contrasenaHash);
        EsAdministrador = esAdministrador;
    }

    protected Usuario()
    {
        Nombre = string.Empty;
        CorreoElectronico = string.Empty;
        ContrasenaHash = string.Empty;
    }

    private Usuario(Guid id, string nombre, string correoElectronico, string contrasenaHash, bool esAdministrador, IEnumerable<Permiso> permisos)
        : this(nombre, correoElectronico, contrasenaHash, esAdministrador)
    {
        if (id == Guid.Empty) throw new DominioException("ID de usuario inválido.");
        if (permisos is null) throw new DominioException("La lista de permisos es obligatoria.");

        Id = id;

        foreach (var permiso in permisos) {
            AsignarPermiso(permiso);
        }
    }

    public static Usuario Reconstruir(Guid id, string nombre, string correoElectronico, string contrasenaHash, bool esAdministrador, IEnumerable<Permiso> permisos)
    {
        return new Usuario(id, nombre, correoElectronico, contrasenaHash, esAdministrador, permisos);
    }

    public void ModificarDatos(string nombre, string correoElectronico)
    {
        Nombre = ValidarNombre(nombre);
        CorreoElectronico = ValidarCorreoElectronico(correoElectronico);
    }

    public void CambiarContrasenaHash(string contrasenaHash)
    {
        ContrasenaHash = ValidarContrasenaHash(contrasenaHash);
    }

    public void AsignarPermiso(Permiso permiso)
    {
        ValidarPermiso(permiso);

        if (!_permisos.Contains(permiso)) {
            _permisos.Add(permiso);
        }
    }

    public void RemoverPermiso(Permiso permiso)
    {
        ValidarPermiso(permiso);
        _permisos.Remove(permiso);
    }

    public void ReemplazarPermisos(IEnumerable<Permiso> permisos)
    {
        if (permisos is null) throw new DominioException("La lista de permisos es obligatoria.");

        _permisos.Clear();

        foreach (var permiso in permisos) {
            AsignarPermiso(permiso);
        }
    }

    public bool PoseePermiso(Permiso permiso)
    {
        ValidarPermiso(permiso);

        if (EsAdministrador) return true;

        if (_permisos.Contains(permiso)) return true;

        return permiso == Permiso.TramiteBaja && _permisos.Contains(Permiso.ExpedienteBaja);
    }

    private static string ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new DominioException("El nombre del usuario es obligatorio.");

        return nombre.Trim();
    }

    private static string ValidarCorreoElectronico(string correoElectronico)
    {
        if (string.IsNullOrWhiteSpace(correoElectronico)) throw new DominioException("El correo electrónico es obligatorio.");

        var correoNormalizado = correoElectronico.Trim();

        if (correoNormalizado.StartsWith('@') || correoNormalizado.EndsWith('@') || !correoNormalizado.Contains('@')) {
            throw new DominioException("El correo electrónico no tiene un formato válido.");
        }

        return correoNormalizado;
    }

    private static string ValidarContrasenaHash(string contrasenaHash)
    {
        if (string.IsNullOrWhiteSpace(contrasenaHash)) throw new DominioException("El hash de la contraseña es obligatorio.");

        return contrasenaHash.Trim();
    }

    private static void ValidarPermiso(Permiso permiso)
    {
        if (!Enum.IsDefined(typeof(Permiso), permiso)) throw new DominioException("El permiso indicado no es válido.");
    }
}
