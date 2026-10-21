import React from 'react';
import { Pressable, StyleSheet, Text, View, useColorScheme } from 'react-native';

import { darkColors, lightColors } from '../../theme/colors';
import { typography } from '../../theme/typography';

type ErrorFallbackProps = {
  onRetry: () => void;
};

export default function ErrorFallback({ onRetry }: ErrorFallbackProps) {
  const colors = useColorScheme() === 'dark' ? darkColors : lightColors;

  return (
    <View style={[styles.container, { backgroundColor: colors.background }]}>
      <Text style={[typography.screenTitle, { color: colors.text }]}>Algo salió mal</Text>
      <Text style={[typography.body, styles.message, { color: colors.textSecondary }]}>
        Tuvimos un problema inesperado. Ya registramos el error, intentá de nuevo.
      </Text>
      <Pressable
        accessibilityRole="button"
        onPress={onRetry}
        style={[styles.button, { backgroundColor: colors.primary }]}
      >
        <Text style={[typography.button, { color: colors.white }]}>Reintentar</Text>
      </Pressable>
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, alignItems: 'center', justifyContent: 'center', padding: 24 },
  message: { textAlign: 'center', marginTop: 12, marginBottom: 24 },
  button: { paddingHorizontal: 24, paddingVertical: 12, borderRadius: 8 },
});
