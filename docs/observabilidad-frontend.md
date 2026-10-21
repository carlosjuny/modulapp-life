# Observabilidad del frontend

## Errores (Sentry)
- DSN por ambiente en `EXPO_PUBLIC_SENTRY_DSN` (`.env` local, no se versiona). Vacío = Sentry desactivado. `sendDefaultPii` está en `false`.
- `src/services/monitoring.ts` inicializa Sentry en `src/app/_layout.tsx`; el layout raíz va envuelto con `Sentry.wrap`.
- Error boundary global: `ErrorBoundary` exportado en `src/app/_layout.tsx` (Expo Router). Reporta el error a Sentry y muestra `ErrorFallback` con el botón Reintentar.
- Mapas de fuente: `metro.config.js` agrega los Debug IDs y el plugin `@sentry/react-native` los sube en el build. Variables del build (EAS o CI, nunca en el repo): `SENTRY_AUTH_TOKEN` (secreto), `SENTRY_ORG`, `SENTRY_PROJECT`. Con `eas update`: `npx sentry-expo-upload-sourcemaps dist`.
- Requiere código nativo: no funciona en Expo Go, usar un development build.
- Verificación: en un build de release, lanzar un error forzado y comprobar que llega a Sentry con la traza legible.
- Sesiones sin fallos (objetivo > 99 %): se mide con Release Health en Sentry.

## Analítica (PostHog)
- Clave por ambiente en `EXPO_PUBLIC_POSTHOG_KEY` y `EXPO_PUBLIC_POSTHOG_HOST`. Sin clave queda desactivada.
- `src/services/analytics.ts` expone `track(evento, propiedades?)`. Eventos del embudo: `registro`, `onboarding_completado`, `primera_tarea`, `invitacion`, `modulo_activado`, `suscripcion`.
- Sin datos personales: el usuario es anónimo (no se llama a `identify`), las propiedades solo admiten `modulo` y `plan`, y `disableGeoip` está activo. Al agregar propiedades nuevas, nunca incluir correos, nombres ni ids de usuario.
- Cada evento se llama con `track(...)` cuando exista su flujo; hoy registro, onboarding, tareas, invitación, módulos y suscripción no tienen pantallas.
