#import "@preview/barcala:0.3.0": apendice, informe, nomenclatura

#show: informe.with(
  unidad-academica: "informática",
  asignatura: "I145 Taller de Lenguaje .NET",
  trabajo: "Fase 1: Dominio Rico y Arquitectura Limpia",
  autores: (
    (
      nombre: "Avila Montoya, Eygleen Fernanda",
      email: "eygleen.avila@alu.ing.unlp.edu.ar",
      legajo: "02931/2",
    ),
    (
      nombre: "Bejarano, Abril",
      email: "abril.bejarano@alu.ing.unlp.edu.ar",
      legajo: "03339/5",
    ),
    (
      nombre: "Tarifa, Armando Ezequiel",
      email: "ezequiel.tarifa.2002@gmail.com",
      legajo: "02893/4",
    ),
  ),

  titulo: [Sistema para la Gestión de Expedientes],
  fecha: "2026-05-19",
)

= Prueba de funcionalidad desde Program.cs

El archivo `Program.cs` contiene las instancias de los repositorios, servicios y casos de uso necesarios, asi como también el código de ejemplo para ambos caminos del programa: el caso en el que todo funciona correctamente y el caso donde se provocan los errores esperados para verificar el manejo de excepciones.

== Configuración inicial

Al inicio del programa se crean las instancias de los repositorios y servicios que utilizarán todos los casos de uso:

\
```cs
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
```

== Camino feliz

=== Agregar un expediente

Se crea un expediente con carátula `"Solicitud de Hardware"`. El caso de uso devuelve el expediente creado con su ID asignado.

\
```cs
var res = agregarExpUseCase.Ejecutar(new AgregarExpedienteRequest("Solicitud de Hardware", miUsuario));
idExpediente = res.Id;
```
\
*Salida esperada:*
```
-> Agregando expediente 'Solicitud de Hardware'...
[OK] Expediente creado. ID: aa6ca6dc-4067-4908-8ed5-0c2ceca4b623
```
=== Agregar trámite PaseAEstudio

Se agrega un trámite con etiqueta `PaseAEstudio`. Esto dispara el servicio de actualización de estado, que cambia el estado del expediente a `ParaResolver`.

\
```cs
agregarTramiteUseCase.Ejecutar(new AgregarTramiteRequest(
    idExpediente,
    EtiquetaTramite.PaseAEstudio,
    "Se evalúa el presupuesto disponible.",
    miUsuario));
```

\
*Salida esperada:*
```
-> Agregando trámite 'PaseAEstudio' (estado debería pasar a ParaResolver)
[OK] Trámite agregado.
```
=== Agregar trámite Resolución

Se agrega un trámite con etiqueta `Resolucion`. El servicio de actualización vuelve a ejecutarse y cambia el estado del expediente a `ConResolucion`.

\
```cs
agregarTramiteUseCase.Ejecutar(new AgregarTramiteRequest(
    idExpediente,
    EtiquetaTramite.Resolucion,
    "Se aprueba la solicitud.",
    miUsuario));
```

\
*Salida esperada:*
```
-> Agregando trámite 'Resolucion' (estado debería pasar a ConResolucion)
[OK] Trámite agregado.
```
=== Listar expedientes

Se listan todos los expedientes registrados para verificar que el estado se haya actualizado correctamente.

\
```cs
var lista = listarExpUseCase.Ejecutar(new ListarTodosLosExpedientesRequest());
foreach (var dto in lista.Expedientes)
{
    Console.WriteLine($"   Carátula: {dto.Caratula} | Estado: {dto.Estado} | ID: {dto.Id}");
}
```

\
*Salida esperada:*
```
-> Listando todos los expedientes
   Carátula: Solicitud de Hardware | Estado: ConResolucion | ID: aa6ca6dc-4067-4908-8ed5-0c2ceca4b623
```
=== Listar trámites del expediente

Se listan los trámites asociados al expediente creado para confirmar que ambos fueron persistidos correctamente.

\
```cs
var tramites = listarTramitesUseCase.Ejecutar(new ListarTramitesPorExpedienteRequest(idExpediente));
foreach (var t in tramites.Tramites)
{
    Console.WriteLine($"   Trámite: {t.Etiqueta} | Contenido: {t.Contenido} | ID: {t.Id}");
}
```

\
*Salida esperada:*
```
-> Listando trámites del expediente
   Trámite: PaseAEstudio | Contenido: Se evalúa el presupuesto disponible. | ID: 6f2e6b5c-c13b-4fbf-86ce-5927e064d600
   Trámite: Resolucion | Contenido: Se aprueba la solicitud. | ID: 5f61bc30-8b0b-48fb-b129-240e32129332
```
=== Cambio de estado manual

Se prueba el cambio de estado manual del expediente, pasándolo a `EnNotificacion`, independientemente de los trámites.

\
```cs
cambiarEstadoUseCase.Ejecutar(new CambiarEstadoExpedienteRequest(
    idExpediente,
    EstadoExpediente.EnNotificacion,
    miUsuario));
```

\
*Salida esperada:*
```
-> Cambiando estado manualmente a 'EnNotificacion'
[OK] Estado cambiado.
```
== Camino triste

En este caso, se prueban situaciones de error para verificar que las excepciones del dominio y de autorización se lanzan y capturan correctamente.

=== Error 1: Carátula vacía

Si se intenta crear un expediente con carátula vacía, el dominio lanza una `DominioException`.

\
```cs
agregarExpUseCase.Ejecutar(new AgregarExpedienteRequest("", miUsuario));
```

\
*Salida esperada:*
```
-> Intentando crear expediente con carátula vacía
[CAPTURADO - DOMINIO]: La carátula no puede estar vacía ni ser nula.
```
=== Error 2: Contenido de trámite vacío

Si se intenta agregar un trámite sin contenido, también se lanza una `DominioException`.

\
```cs
agregarTramiteUseCase.Ejecutar(new AgregarTramiteRequest(
    idExpediente,
    EtiquetaTramite.Despacho,
    "",
    miUsuario));
```

\
*Salida esperada:*
```
-> Intentando agregar trámite con contenido vacío
[CAPTURADO - DOMINIO]: El contenido del trámite es obligatorio y no puede estar vacío.
```
=== Error 3: Expediente inexistente

Si se intenta modificar la carátula de un expediente que no existe (usando un `Guid` aleatorio), se lanza una `DominioException`.

\
```cs
modificarCaratulaUseCase.Ejecutar(new ModificarCaratulaExpedienteRequest(
    Guid.NewGuid(),
    "Nueva carátula",
    miUsuario));
```
\
*Salida esperada:*
```
-> Intentando modificar carátula de un expediente inexistente
[CAPTURADO - DOMINIO]: No se encontró el expediente solicitado.
```

=== Error 4: Autorización denegada

Para probar el caso de `AutorizacionException`, se debe ir a la clase `AutorizacionProvisionalService` y cambiar el `return true` por `return false`. Al hacerlo, cualquier operación que requiera autorización lanzará esta excepción.

\
```cs
// En AutorizacionProvisionalService:
public bool PoseeElPermiso(Guid usuario, Permiso permiso) => false;
```

*Salida esperada (al intentar agregar un expediente, por ejemplo):*
```
[CAPTURADO - AUTORIZACION]: Usuario no autorizado para agregar expedientes.
```
== Salida completa de la consola

Esta es la salida completa que se espera después de ejecutar `Program.cs` con `AutorizacionProvisionalService` en `return true`:

```
CAMINO FELIZ :) : 

SGE - SISTEMA DE GESTION DE EXPEDIENTES
-> Agregando expediente 'Solicitud de Hardware'...
[OK] Expediente creado. ID: aa6ca6dc-4067-4908-8ed5-0c2ceca4b623

-> Agregando trámite 'PaseAEstudio' (estado debería pasar a ParaResolver)
[OK] Trámite agregado.

-> Agregando trámite 'Resolucion' (estado debería pasar a ConResolucion)
[OK] Trámite agregado.

-> Listando todos los expedientes
   Carátula: Solicitud de Hardware | Estado: ConResolucion | ID: aa6ca6dc-4067-4908-8ed5-0c2ceca4b623

-> Listando trámites del expediente
   Trámite: PaseAEstudio | Contenido: Se evalúa el presupuesto disponible. | ID: 6f2e6b5c-c13b-4fbf-86ce-5927e064d600
   Trámite: Resolucion | Contenido: Se aprueba la solicitud. | ID: 5f61bc30-8b0b-48fb-b129-240e32129332

-> Cambiando estado manualmente a 'EnNotificacion'
[OK] Estado cambiado.

CAMINO TRISTE :( : 

-> Intentando crear expediente con carátula vacía
[CAPTURADO - DOMINIO]: La carátula no puede estar vacía ni ser nula.

-> Intentando agregar trámite con contenido vacío
[CAPTURADO - DOMINIO]: El contenido del trámite es obligatorio y no puede estar vacío.

-> Intentando modificar carátula de un expediente inexistente
[CAPTURADO - DOMINIO]: No se encontró el expediente solicitado.

-> (Para probar AutorizacionException: cambiar AutorizacionProvisionalService a return false)
   Al hacerlo, cualquier operación lanzará AutorizacionException.

FIN DE LA DEMO
```
