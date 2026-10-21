# Observabilidad del backend

## Logs (Serilog)
- Ambiente `Development`: texto en consola con `[{CorrelationId}]`. Otros ambientes: JSON compacto (`RenderedCompactJsonFormatter`) a consola.
- Cada petición recibe `X-Correlation-ID` (se acepta el del cliente si es alfanumérico/`-`/`_` y de hasta 64 caracteres; si no, se genera). Se devuelve en la respuesta, entra en cada log como `CorrelationId`, en las respuestas 500 (`correlationId`) y como tag `correlation_id` en Sentry.
- Se registra una línea por petición (método, ruta sin query string, status, duración). No se loguean cuerpos, cabeceras ni query strings. Al escribir logs nuevos, nunca incluir contraseñas, tokens ni datos personales.
- Niveles: sección `Serilog:MinimumLevel` de `appsettings*.json`.

## Errores (Sentry)
- DSN por ambiente en `Sentry__Dsn` (variable de entorno o user-secrets). Vacío = Sentry desactivado. `SendDefaultPii` está en `false`.
- Se envían los logs de nivel `Error` o superior, incluidas las excepciones no controladas (las registra `GlobalExceptionHandler`).

## Métricas
- `GET /metrics` (formato Prometheus, OpenTelemetry). Se desactiva con `Observability__MetricsEnabled=false`.
- Latencia p95: `histogram_quantile(0.95, sum by (le) (rate(http_server_request_duration_seconds_bucket[5m])))` (objetivo < 0,5 s).
- Tasa de errores: `sum(rate(http_server_request_duration_seconds_count{http_response_status_code=~"5.."}[5m])) / sum(rate(http_server_request_duration_seconds_count[5m]))`.
- `/metrics` no tiene autenticación: en staging/producción restringirlo a la red interna o al proxy del scraper.

## Alerta de disponibilidad (objetivo 99,5 % mensual)
Se configura fuera del código, en la herramienta de monitoreo del ambiente: sondeo a `GET /health` cada minuto (200 = Healthy, 503 = Unhealthy) y alerta cuando la disponibilidad mensual baje de 99,5 %.
