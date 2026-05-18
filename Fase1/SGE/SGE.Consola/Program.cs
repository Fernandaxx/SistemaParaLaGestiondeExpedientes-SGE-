using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Tramites;
using SGE.Dominio.Comun;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Infraestructura.Autorizacion;
using SGE.Infraestructura.Expedientes;
using SGE.Infraestructura.Tramites;

IExpedienteRepository expedienteRepo = new ExpedienteTxtRepository();
ITramiteRepository tramiteRepo = new TramiteTxtRepository();
IAutorizacionService authService = new AutorizacionProvisionalService();

var actualizacionService = new ActualizacionEstadoExpedienteService(expedienteRepo, tramiteRepo);

var agregarExpUseCase = new AgregarExpedienteUseCase(expedienteRepo, authService);
var listarExpUseCase = new ListarTodosLosExpedientesUseCase(expedienteRepo);
var modificarCaratulaUseCase = new ModificarCaratulaExpedienteUseCase(expedienteRepo, authService);
var cambiarEstadoUseCase = new CambiarEstadoExpedienteUseCase(expedienteRepo, authService);
var eliminarExpUseCase = new EliminarExpedienteUseCase(expedienteRepo, tramiteRepo, authService);
var agregarTramiteUseCase = new AgregarTramiteUseCase(tramiteRepo, authService, actualizacionService);
var listarTramitesUseCase = new ListarTramitesPorExpedienteUseCase(tramiteRepo);
var modificarTramiteUseCase = new ModificarTramiteUseCase(tramiteRepo, authService, actualizacionService);
var eliminarTramiteUseCase = new EliminarTramiteUseCase(tramiteRepo, authService, actualizacionService);

Guid miUsuario = Guid.NewGuid();

Console.WriteLine("CAMINO FELIZ :) : \n");

Console.WriteLine("      SGE - SISTEMA DE GESTIÓN DE EXPEDIENTES");


// 1. Agregar expediente
Guid idExpediente = Guid.Empty;
try
{
    Console.WriteLine("-> Agregando expediente 'Solicitud de Hardware'...");
    var res = agregarExpUseCase.Ejecutar(new AgregarExpedienteRequest("Solicitud de Hardware", miUsuario));
    idExpediente = res.Id;
    Console.WriteLine($"[OK] Expediente creado. ID: {idExpediente}\n");
}
catch (DominioException ex) { Console.WriteLine($"[ERROR DOMINIO]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[ERROR AUTORIZACION]: {ex.Message}\n"); }
catch (Exception ex) { Console.WriteLine($"[ERROR GENERAL]: {ex.Message}\n"); }

// 2. Agregar tramite PaseAEstudio -> estado pasa a ParaResolver
try
{
    Console.WriteLine("-> Agregando trámite 'PaseAEstudio' (estado debería pasar a ParaResolver)...");
    agregarTramiteUseCase.Ejecutar(new AgregarTramiteRequest(idExpediente, EtiquetaTramite.PaseAEstudio, "Se evalúa el presupuesto disponible.", miUsuario));
    Console.WriteLine("[OK] Trámite agregado.\n");
}
catch (DominioException ex) { Console.WriteLine($"[ERROR DOMINIO]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[ERROR AUTORIZACION]: {ex.Message}\n"); }
catch (Exception ex) { Console.WriteLine($"[ERROR GENERAL]: {ex.Message}\n"); }

// 3. Agregar tramite Resolucion -> estado pasa a ConResolucion
try
{
    Console.WriteLine("-> Agregando trámite 'Resolucion' (estado debería pasar a ConResolucion)...");
    agregarTramiteUseCase.Ejecutar(new AgregarTramiteRequest(idExpediente, EtiquetaTramite.Resolucion, "Se aprueba la solicitud.", miUsuario));
    Console.WriteLine("[OK] Trámite agregado.\n");
}
catch (DominioException ex) { Console.WriteLine($"[ERROR DOMINIO]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[ERROR AUTORIZACION]: {ex.Message}\n"); }
catch (Exception ex) { Console.WriteLine($"[ERROR GENERAL]: {ex.Message}\n"); }

// 4. Listar expedientes y ver el estado actualizado
try
{
    Console.WriteLine("-> Listando todos los expedientes...");
    var lista = listarExpUseCase.Ejecutar(new ListarTodosLosExpedientesRequest());
    foreach (var dto in lista.Expedientes)
    {
        Console.WriteLine($"   Carátula: {dto.Caratula} | Estado: {dto.Estado} | ID: {dto.Id}");
    }
    Console.WriteLine();
}
catch (Exception ex) { Console.WriteLine($"[ERROR GENERAL]: {ex.Message}\n"); }

// 5. Listar tramites del expediente
try
{
    Console.WriteLine("-> Listando trámites del expediente...");
    var tramites = listarTramitesUseCase.Ejecutar(new ListarTramitesPorExpedienteRequest(idExpediente));
    foreach (var t in tramites.Tramites)
    {
        Console.WriteLine($"   Trámite: {t.Etiqueta} | Contenido: {t.Contenido} | ID: {t.Id}");
    }
    Console.WriteLine();
}
catch (Exception ex) { Console.WriteLine($"[ERROR GENERAL]: {ex.Message}\n"); }

// 6. Cambio de estado manual
try
{
    Console.WriteLine("-> Cambiando estado manualmente a 'EnNotificacion'...");
    cambiarEstadoUseCase.Ejecutar(new CambiarEstadoExpedienteRequest(idExpediente, EstadoExpediente.EnNotificacion, miUsuario));
    Console.WriteLine("[OK] Estado cambiado.\n");
}
catch (DominioException ex) { Console.WriteLine($"[ERROR DOMINIO]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[ERROR AUTORIZACION]: {ex.Message}\n"); }
catch (Exception ex) { Console.WriteLine($"[ERROR GENERAL]: {ex.Message}\n"); }


Console.WriteLine("CAMINO TRISTE :( : \n");

// Error 1: carátula vacía -> DominioException
try
{
    Console.WriteLine("-> Intentando crear expediente con carátula vacía...");
    agregarExpUseCase.Ejecutar(new AgregarExpedienteRequest("", miUsuario));
}
catch (DominioException ex) { Console.WriteLine($"[CAPTURADO - DOMINIO]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[CAPTURADO - AUTORIZACION]: {ex.Message}\n"); }
catch (Exception ex) { Console.WriteLine($"[ERROR GENERAL]: {ex.Message}\n"); }

// Error 2: contenido de tramite vacío -> DominioException
try
{
    Console.WriteLine("-> Intentando agregar trámite con contenido vacío...");
    agregarTramiteUseCase.Ejecutar(new AgregarTramiteRequest(idExpediente, EtiquetaTramite.Despacho, "", miUsuario));
}
catch (DominioException ex) { Console.WriteLine($"[CAPTURADO - DOMINIO]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[CAPTURADO - AUTORIZACION]: {ex.Message}\n"); }
catch (Exception ex) { Console.WriteLine($"[ERROR GENERAL]: {ex.Message}\n"); }

// Error 3: expediente inexistente -> DominioException
try
{
    Console.WriteLine("-> Intentando modificar carátula de un expediente inexistente...");
    modificarCaratulaUseCase.Ejecutar(new ModificarCaratulaExpedienteRequest(Guid.NewGuid(), "Nueva carátula", miUsuario));
}
catch (DominioException ex) { Console.WriteLine($"[CAPTURADO - DOMINIO]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[CAPTURADO - AUTORIZACION]: {ex.Message}\n"); }
catch (Exception ex) { Console.WriteLine($"[ERROR GENERAL]: {ex.Message}\n"); }

// Error 4: AutorizacionException (cambiar AutorizacionProvisionalService a false manualmente para probar)
// Para probarlo: ir a AutorizacionProvisionalService y cambiar return true por return false
// El resultado esperado es:
// [CAPTURADO - AUTORIZACION]: Usuario no autorizado para agregar expedientes.

Console.WriteLine("-> (Para probar AutorizacionException: cambiar AutorizacionProvisionalService a return false)");
Console.WriteLine("   Al hacerlo, cualquier operación lanzará AutorizacionException.\n");

Console.WriteLine("              FIN DE LA DEMO");