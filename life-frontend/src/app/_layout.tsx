import { useEffect } from 'react';
import { Stack, type ErrorBoundaryProps } from 'expo-router';
import { SafeAreaProvider } from 'react-native-safe-area-context';

import ErrorFallback from '../components/organisms/ErrorFallback';
import { ThemeProvider } from '../context/ThemeContext';
import { initMonitoring, reportError, wrapWithMonitoring } from '../services/monitoring';

initMonitoring();

export function ErrorBoundary({ error, retry }: ErrorBoundaryProps) {
  useEffect(() => {
    reportError(error);
  }, [error]);

  return <ErrorFallback onRetry={retry} />;
}

function RootLayout() {
  return (
    <SafeAreaProvider>
      <ThemeProvider>
        <Stack screenOptions={{ headerShown: false }} />
      </ThemeProvider>
    </SafeAreaProvider>
  );
}

export default wrapWithMonitoring(RootLayout);
