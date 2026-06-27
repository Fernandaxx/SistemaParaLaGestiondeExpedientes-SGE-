# Sistema para la Gestion de Expedientes (SGE)

Trabajo Practico 2 / Fase 2 - Seminario de Lenguajes opcion .NET.

La entrega final esta en `Fase2/SGE`. La carpeta `Fase1/SGE` se conserva como registro de la primera entrega.

## Estructura

La solucion de Fase 2 esta organizada en cuatro proyectos:

- `SGE.Dominio`: entidades, value objects, reglas de dominio y permisos.
- `SGE.Aplicacion`: casos de uso, contratos de repositorios, autorizacion, hash, token y unidad de trabajo.
- `SGE.Infraestructura`: EF Core, SQLite, repositorios reales, seed, hash, JWT y autorizacion real.
- `SGE.WebApi`: Minimal API, composition root, JWT, Scalar y manejo global de errores.

## Requisitos

- .NET 10 SDK.
- SQLite se usa mediante EF Core. No hace falta crear la base a mano.

## Compilar y probar

Desde la carpeta de la solucion:

```bash
cd Fase2/SGE
dotnet restore
dotnet build
dotnet test
```

Nota: actualmente no hay un proyecto de tests automatizados. `dotnet test` valida restore/build de la solucion.

Puede aparecer el warning `NU1903` por `SQLitePCLRaw.lib.e_sqlite3`, dependencia transitiva de `Microsoft.EntityFrameworkCore.Sqlite`.

## Ejecutar la WebApi

Desde `Fase2/SGE`:

```bash
dotnet run --project SGE.WebApi
```

Por defecto, el perfil HTTP usa:

```text
http://localhost:5073
```

Scalar queda disponible en:

```text
http://localhost:5073/scalar/v1
```

El documento OpenAPI queda disponible en:

```text
http://localhost:5073/openapi/v1.json
```

## Base de datos

La base se crea automaticamente con `EnsureCreated()` al iniciar la WebApi.

Ruta esperada:

```text
Fase2/SGE/SGE.WebApi/SGE.sqlite
```

Tambien se configura `PRAGMA journal_mode=DELETE`.

## Usuarios semilla

La base inicial incluye estos usuarios:

| Usuario | Contrasena | Tipo | Permisos |
| --- | --- | --- | --- |
| `admin@sge.com` | `admin123` | Administrador | Todos los permisos por regla de administrador |
| `operador@sge.com` | `operador123` | Usuario de prueba | `ExpedienteAlta`, `ExpedienteModificacion`, `TramiteAlta`, `TramiteModificacion` |
| `lector@sge.com` | `lector123` | Usuario de prueba | Sin permisos de mutacion |

Las contrasenas se guardan hasheadas, no en texto plano.

## Autenticacion en Scalar

1. Abrir `http://localhost:5073/scalar/v1`.
2. Ejecutar `POST /auth/login`.
3. Copiar el valor `token` de la respuesta.
4. En Scalar, cargarlo como token Bearer.
5. Ejecutar endpoints protegidos.

Todas las rutas de negocio requieren token. Si falta el token, devuelven `401 Unauthorized`.

## Endpoints principales

### Autenticacion

`POST /auth/registrar`

```json
{
  "nombre": "Usuario Nuevo",
  "correoElectronico": "nuevo@sge.com",
  "contrasena": "nuevo123"
}
```

`POST /auth/login`

```json
{
  "correoElectronico": "admin@sge.com",
  "contrasena": "admin123"
}
```

### Usuarios

`GET /usuarios` - requiere administrador.

`PUT /usuarios/me` - usuario autenticado modifica sus datos. El `UserId` sale del token, no del body.

```json
{
  "nombre": "Nombre Actualizado",
  "correoElectronico": "actualizado@sge.com",
  "contrasena": "otra123"
}
```

`PUT /usuarios/{id}/permisos` - requiere administrador.

```json
{
  "permisos": [
    "ExpedienteAlta",
    "TramiteAlta"
  ]
}
```

`DELETE /usuarios/{id}` - requiere administrador.

### Expedientes

`GET /expedientes`

`GET /expedientes/{id}` - devuelve el expediente con sus tramites.

`POST /expedientes`

```json
{
  "caratula": "Expediente de prueba"
}
```

`PUT /expedientes/{id}/caratula`

```json
{
  "caratula": "Nueva caratula"
}
```

`PUT /expedientes/{id}/estado`

```json
{
  "estado": "ParaResolver"
}
```

Estados validos:

```text
RecienIniciado
ParaResolver
ConResolucion
EnNotificacion
Finalizado
```

`DELETE /expedientes/{id}` - elimina el expediente y sus tramites asociados.

### Tramites

`GET /expedientes/{expedienteId}/tramites`

`POST /expedientes/{expedienteId}/tramites`

```json
{
  "etiqueta": "Resolucion",
  "contenido": "Contenido del tramite"
}
```

`PUT /tramites/{id}`

```json
{
  "etiqueta": "Notificacion",
  "contenido": "Contenido actualizado"
}
```

`DELETE /tramites/{id}`

Etiquetas validas:

```text
EscritoPresentado
PaseAEstudio
Despacho
Resolucion
Notificacion
PaseAlArchivo
```

## Orden recomendado para probar desde Scalar

1. Ejecutar `POST /auth/login` con `admin@sge.com` / `admin123`.
2. Cargar el token admin como Bearer.
3. Ejecutar `GET /usuarios` y verificar que devuelve los tres usuarios semilla.
4. Ejecutar `GET /usuarios` sin token y verificar `401 Unauthorized`.
5. Ejecutar `POST /auth/login` con `lector@sge.com` / `lector123`.
6. Cargar el token lector e intentar `POST /expedientes`; debe devolver `403 Forbidden`.
7. Volver al token admin.
8. Crear un expediente con `POST /expedientes`.
9. Crear un tramite `Resolucion` con `POST /expedientes/{expedienteId}/tramites`.
10. Consultar `GET /expedientes/{id}` y verificar que el estado sea `ConResolucion` y que incluya el tramite.
11. Eliminar el tramite con `DELETE /tramites/{id}`.
12. Consultar otra vez `GET /expedientes/{id}` y verificar que no tenga tramites y que el estado vuelva a `RecienIniciado`.
13. Crear otro expediente con un tramite y eliminar el expediente con `DELETE /expedientes/{id}` para probar baja en cascada.
14. Consultar `GET /expedientes/{id}` sobre el expediente eliminado y verificar `404 Not Found`.

## Manejo de errores

La WebApi usa `ProblemDetails`:

- `DominioException` -> `400 Bad Request`.
- `AutorizacionException` -> `403 Forbidden`.
- `EntidadNoEncontradaException` -> `404 Not Found`.
- `RepositorioException` -> `500 Internal Server Error`.

## Notas de seguridad

- El login devuelve un token JWT con el `UserId`.
- Las operaciones protegidas extraen el usuario desde `ClaimsPrincipal`.
- Ningun endpoint protegido recibe `UserId` por body.
- Los repositorios EF Core no llaman a `SaveChanges`; los casos de uso mutativos llaman a `IUnidadDeTrabajo.Guardar()`.
