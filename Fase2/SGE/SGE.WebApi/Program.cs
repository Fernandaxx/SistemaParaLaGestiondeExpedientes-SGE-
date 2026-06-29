using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SGE.Aplicacion;
using SGE.Aplicacion.Autorizacion;
using SGE.Infraestructura;
using SGE.Infraestructura.Datos;
using SGE.WebApi.Endpoints;
using SGE.WebApi.Errores;
using SGE.WebApi.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<JwtBearerSecurityDocumentTransformer>();
    options.AddOperationTransformer<JwtBearerSecurityOperationTransformer>();
});

builder.Services.AddAplicacion();

// --- CORRECCIÓN ACÁ ---
var connectionString = builder.Configuration.GetConnectionString("SgeDb")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión SgeDb.");

// Pasamos la cadena limpia directo, como dicta la Teoría 12
builder.Services.AddInfraestructura(connectionString);
// ----------------------

var jwtClave = builder.Configuration["Jwt:Clave"]
    ?? throw new InvalidOperationException("No se encontró la configuración Jwt:Clave.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Emisor"] ?? "SGE.WebApi",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audiencia"] ?? "SGE.WebApi",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtClave)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SgeContext>();
    var hashService = scope.ServiceProvider.GetRequiredService<IHashService>();
    SgeSqlite.Inicializar(context, hashService);
}

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapEstadoEndpoints();
app.MapAutenticacionEndpoints();
app.MapUsuariosEndpoints();
app.MapExpedientesEndpoints();
app.MapTramitesEndpoints();

app.Run();