namespace SGE.Aplicacion.Autorizacion;

public record class LoginResponse(Guid UserId, string Token);
