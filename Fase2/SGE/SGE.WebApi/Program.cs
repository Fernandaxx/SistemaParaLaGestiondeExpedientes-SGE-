using SGE.Aplicacion;
using SGE.Infraestructura;
using SGE.WebApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAplicacion();
builder.Services.AddInfraestructura();

var app = builder.Build();

app.MapEstadoEndpoints();
app.MapAutenticacionEndpoints();
app.MapUsuariosEndpoints();
app.MapExpedientesEndpoints();
app.MapTramitesEndpoints();

app.Run();
