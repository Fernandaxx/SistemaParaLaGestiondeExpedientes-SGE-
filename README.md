# SGE - Sistema para la Gestion de Expedientes
**Integrantes**: 

| Nombre | Apellido | Legajo |
|:----:|:---:|:---:|
| Avila Montoya | Eygleen Fernanda | 02931/2 |
| Bejarano | Abril | 03339/5 |
| Tarifa | Armando Ezequiel | 02893/4 |

Trabajo Practico 2 - Seminario de Lenguajes opcion .NET, 1er semestre 2026.

La carpeta `Fase1/SGE` se deja como referencia de la primera entrega. Lo de esta entrega se encuentra en `Fase2/SGE`.

## Proyectos de la solucion

- `SGE.Dominio`: entidades (Expediente, Tramite, Usuario), value objects y reglas de negocio.
- `SGE.Aplicacion`: casos de uso, contratos de repositorios (interfaces), unidad de trabajo.
- `SGE.Infraestructura`: EF Core + SQLite, repositorios reales, hash de contraseñas, JWT, autorizacion y carga de usuarios semilla.
- `SGE.WebApi`: Minimal API. Acá esta el Program.cs que arma todo y los endpoints.

## Como correrlo

```bash
cd Fase2/SGE/SGE.WebApi
dotnet run
```

La consola va a mostrar `Now listening on: http://localhost:5073`. Con esa URL + `/scalar/v1` se abre la documentacion interactiva, por ejemplo:

```
http://localhost:5073/scalar/v1
```

La primera vez que arranca, crea sola la base `SGE.sqlite` en esa misma carpeta (`SGE.WebApi/`) con los usuarios semilla ya cargados.

## Usuarios que ya vienen cargados

| Correo | Contraseña | Es admin | Permisos |
| --- | --- | --- | --- |
| admin@sge.com | admin123 | Si | Todos (por ser admin tiene acceso total) |
| operador@sge.com | operador123 | No | ExpedienteAlta, ExpedienteModificacion, TramiteAlta, TramiteModificacion |
| lector@sge.com | lector123 | No | Ninguno (solo puede loguearse) |

Las contraseñas de la tabla son las que hay que usar para loguearse desde Scalar. En la base se guarda el hash (SHA-256), nunca la contraseña en texto plano.

## Como probar desde Scalar

1. Ejecutar `POST /auth/login` con `admin@sge.com` / `admin123`.
2. Cargar el token admin como Bearer.
3. Ejecutar `GET /usuarios` y verificar que devuelve los tres usuarios semilla.
4. Ejecutar `GET /usuarios` sin token y verificar `401 Unauthorized`.
5. Ejecutar `POST /auth/login` con `lector@sge.com` / `lector123` y copiar el token.
6. Cargar el token del lector e intentar `POST /expedientes`; debe devolver la `403 Forbidden`.
7. Volver a loguearse con el admin y dejar cargado su token.
8. Crear un expediente con `POST /expedientes` y guardar el id que devuelve.
9. Crear un tramite `Resolucion` con `POST /expedientes/{expedienteId}/tramites`; completar con el id del paso anterior y guardar el id de tramite.
10. Consultar `GET /expedientes/{id}`, con el id del paso 8, y verificar que el estado sea `ConResolucion` y que incluya el tramite.
11. Eliminar el tramite con `DELETE /tramites/{id}`, pegar el id generado en 9.
12. Consultar otra vez `GET /expedientes/{id}` y verificar que no tenga tramites y que el estado vuelva a `RecienIniciado`.
13. Crear otro expediente con un tramite y eliminar el expediente con `DELETE /expedientes/{id}` para probar baja en cascada.
14. Consultar `GET /expedientes/{id}` sobre el expediente eliminado y verificar `404 Not Found`.

## Endpoints

### Auth (publicos, no requieren token)

- `POST /auth/registrar` -> `{ "nombre", "correoElectronico", "contrasena" }`
- `POST /auth/login` -> `{ "correoElectronico", "contrasena" }`, devuelve `{ "userId", "token" }`

### Usuarios (requieren token)

- `GET /usuarios` - solo admin.
- `PUT /usuarios/me` - cualquier usuario logueado modifica sus propios datos.
- `PUT /usuarios/{id}/permisos` - solo admin.
- `DELETE /usuarios/{id}` - solo admin.

### Expedientes (requieren token)

- `GET /expedientes`
- `GET /expedientes/{id}` - incluye los tramites del expediente
- `POST /expedientes` -> `{ "caratula" }`
- `PUT /expedientes/{id}/caratula` -> `{ "caratula" }`
- `PUT /expedientes/{id}/estado` -> `{ "estado" }` (valores: RecienIniciado, ParaResolver, ConResolucion, EnNotificacion, Finalizado)
- `DELETE /expedientes/{id}` - borra el expediente y sus tramites

### Tramites (requieren token)

- `GET /expedientes/{expedienteId}/tramites`
- `POST /expedientes/{expedienteId}/tramites` -> `{ "etiqueta", "contenido" }` (etiquetas: EscritoPresentado, PaseAEstudio, Despacho, Resolucion, Notificacion, PaseAlArchivo)
- `PUT /tramites/{id}` -> `{ "etiqueta", "contenido" }`
- `DELETE /tramites/{id}`

## Manejo de errores

Las excepciones de negocio se traducen a respuestas `ProblemDetails`:

- `DominioException` -> 400
- `AutorizacionException` -> 403
- `EntidadNoEncontradaException` -> 404
- `RepositorioException` (algo raro a nivel base de datos) -> 500