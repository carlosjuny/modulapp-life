import { useEffect, useState } from 'react';
import { ActivityIndicator, Pressable } from 'react-native';
import { router } from 'expo-router';

import ModulLayout from '../../components/atoms/ModulLayout';
import ModulView from '../../components/atoms/ModulView';
import ModulText from '../../components/atoms/ModulText';
import { useTheme } from '../../context/ThemeContext';
import { ApiError, getHealth } from '../../services/api';

type State =
  | { status: 'loading' }
  | { status: 'ok'; value: string }
  | { status: 'error'; message: string };

export default function Diagnostico() {
  const { colors } = useTheme();
  const [state, setState] = useState<State>({ status: 'loading' });
  const [attempt, setAttempt] = useState(0);

  useEffect(() => {
    let active = true;

    getHealth()
      .then((value) => {
        if (active) setState({ status: 'ok', value });
      })
      .catch((error) => {
        if (!active) return;

        setState({
          status: 'error',
          message:
            error instanceof ApiError
              ? error.message
              : 'Ocurrió un error inesperado.',
        });
      });

    return () => {
      active = false;
    };
  }, [attempt]);

  const retry = () => {
    setState({ status: 'loading' });
    setAttempt((n) => n + 1);
  };

  const healthy = state.status === 'ok' && state.value === 'Healthy';

  return (
    <ModulLayout
      header={{
        title: 'Diagnóstico',
        leftAction: 'back',
        showNotifications: false,
        onBackPress: () => router.back(),
      }}
    >
      <ModulView p16>
        <ModulText variant="sectionTitle">
          Estado de la API
        </ModulText>

        {state.status === 'loading' && (
          <ActivityIndicator color={colors.primary} />
        )}

        {state.status === 'ok' && (
          <ModulText
            variant="screenTitle"
            style={{
              color: healthy ? colors.success : colors.danger,
            }}
          >
            {healthy ? 'Healthy' : 'Unhealthy'}
          </ModulText>
        )}

        {state.status === 'error' && (
          <ModulText style={{ color: colors.danger }}>
            {state.message}
          </ModulText>
        )}

        <Pressable
          onPress={retry}
          accessibilityRole="button"
          accessibilityLabel="Reintentar"
        >
          <ModulText variant="button">
            Reintentar
          </ModulText>
        </Pressable>
      </ModulView>
    </ModulLayout>
  );
}
