using System.Security.Claims;
using SGE.Aplicacion.Autorizacion;

namespace SGE.WebApi.Comun;

public static class ClaimsPrincipalExtensions
{
    public static Guid ObtenerUserId(this ClaimsPrincipal user)
    {
        var valor = user.FindFirst("UserId")?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(valor, out var userId) || userId == Guid.Empty)
            throw new AutorizacionException("No se pudo identificar al usuario autenticado.");

        return userId;
    }
}
