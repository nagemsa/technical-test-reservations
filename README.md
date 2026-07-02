# Technical Test Reservations API

Servicio backend para reservas de traslados en .NET 8, organizado con una separación hexagonal simple: el dominio concentra la regla de negocio, la aplicación coordina los casos de uso y la API se limita a traducir HTTP.

## Alcance

La solución cubre los cinco endpoints pedidos por la prueba: crear, listar, consultar por id, confirmar y cancelar reservas. Las consultas `GET` operan directamente sobre el repositorio en memoria; no agregué una capa de caché porque el enunciado no la pide y, en esta versión, no aportaría valor real sobre una fuente ya efímera.

## Comportamiento funcional

- Validación de campos obligatorios.
- Pasajeros entre 1 y 6.
- Fecha futura y distinta al pasado.
- Origen y destino diferentes.
- Detección de reservas duplicadas por cliente, origen, destino, fecha y tipo.
- Cálculo de precio con tarifa base, recargo por pasajero y ajustes por fecha y tamaño del grupo.

## Estructura

- `src/TechnicalTest.Api`: contratos HTTP, controladores y mapeo de respuestas.
- `src/TechnicalTest.Application`: casos de uso, errores tipados y puertos.
- `src/TechnicalTest.Domain`: entidad de reserva, estados y pricing.
- `src/TechnicalTest.Infrastructure`: adaptadores en memoria y reloj del sistema.
- `tests/TechnicalTest.Tests`: pruebas unitarias de pricing y flujo de negocio.
- `Documentos`: lectura complementaria y traducción conceptual a Spring Boot.

## Ejecución

```powershell
dotnet restore
dotnet test
dotnet run --project src/TechnicalTest.Api
```

## Supuestos

La fecha entra como UTC, el almacenamiento es solo en memoria y la solución no introduce persistencia real porque no forma parte del alcance. La comparación de duplicados normaliza espacios y no distingue mayúsculas o minúsculas.

## Evolución natural

### Mejoras de backend

- **Idempotencia**: Implementar manejo de solicitudes idempotentes para detectar duplicados y evitar efectos secundarios.
- **Manejo de concurrencia**: Bloqueo optimista o versiones en registros para transiciones de estado seguras.
- **Autenticación y autorización**: Control de acceso basado en JWT y políticas de roles.
- **Paginación y filtrado**: Soporte para conjuntos de resultados paginados y consultas avanzadas.
- **Versionado de API**: Estrategia de versiones (v1, v2) para compatibilidad hacia atrás.
- **Manejo de errores mejorado**: Respuestas estructuradas con detalles del problema.
- **Rate limiting**: Limitación de velocidad por usuario o clave de API.

### Consideraciones de infraestructura

- Persistencia en base de datos (reemplazando repositorio en memoria).
- Estrategia de caché distribuido.
- Observabilidad (monitoreo, logging, tracing).
- Balanceo de carga y escalabilidad horizontal.
- API Gateway para gestionar aspectos transversales.
