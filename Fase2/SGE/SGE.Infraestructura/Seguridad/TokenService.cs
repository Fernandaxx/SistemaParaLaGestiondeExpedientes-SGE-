using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SGE.Aplicacion.Autorizacion;
using SGE.Dominio.Usuarios;

namespace SGE.Infraestructura.Seguridad;

public class TokenService(IConfiguration _configuration) : ITokenService
{
    public string GenerarToken(Usuario usuario)
    {
        var clave = _configuration["Jwt:Clave"]
            ?? throw new InvalidOperationException("No se encontró la configuración Jwt:Clave.");

        var emisor = _configuration["Jwt:Emisor"] ?? "SGE.WebApi";
        var audiencia = _configuration["Jwt:Audiencia"] ?? "SGE.WebApi";
        var duracionMinutos = int.TryParse(_configuration["Jwt:DuracionMinutos"], out var duracion)
            ? duracion
            : 60;

        var claims = new List<Claim>
        {
            new("UserId", usuario.Id.ToString()),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Email, usuario.CorreoElectronico),
            new(ClaimTypes.Name, usuario.Nombre)
        };

        if (usuario.EsAdministrador) {
            claims.Add(new Claim(ClaimTypes.Role, "Administrador"));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: emisor,
            audience: audiencia,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(duracionMinutos),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
