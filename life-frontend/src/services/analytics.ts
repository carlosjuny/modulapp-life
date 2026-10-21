import PostHog from 'posthog-react-native';

const API_KEY = process.env.EXPO_PUBLIC_POSTHOG_KEY;
const HOST = process.env.EXPO_PUBLIC_POSTHOG_HOST ?? 'https://us.i.posthog.com';

export type AnalyticsEvent =
  | 'registro'
  | 'onboarding_completado'
  | 'primera_tarea'
  | 'invitacion'
  | 'modulo_activado'
  | 'suscripcion';

type EventProperties = Partial<Record<'modulo' | 'plan', string>>;

const client = new PostHog(API_KEY ?? 'sin-clave', {
  host: HOST,
  disabled: !API_KEY,
  disableGeoip: true,
  captureAppLifecycleEvents: false,
});

export function track(event: AnalyticsEvent, properties?: EventProperties) {
  client.capture(event, properties);
}
