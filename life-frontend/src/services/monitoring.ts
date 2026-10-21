import * as Sentry from '@sentry/react-native';

const DSN = process.env.EXPO_PUBLIC_SENTRY_DSN;
const APP_ENV = process.env.EXPO_PUBLIC_APP_ENV ?? 'dev';

export function initMonitoring() {
  Sentry.init({
    dsn: DSN,
    enabled: Boolean(DSN),
    environment: APP_ENV,
    sendDefaultPii: false,
  });
}

export function reportError(error: unknown) {
  Sentry.captureException(error);
}

export const wrapWithMonitoring = Sentry.wrap;
