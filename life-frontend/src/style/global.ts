
import { StyleSheet } from 'react-native';
import type { ThemeColors } from '../theme/colors';

export const createGlobalStyles = (colors: ThemeColors) =>
  StyleSheet.create({
    // Dimensions
    w100: { width: '100%' },
    w50: { width: '50%' },
    h100: { height: 100 },
    hFull: { height: '100%' },

    // Spacing
    p0: { padding: 0 },
    p8: { padding: 8 },
    p16: { padding: 16 },
    p24: { padding: 24 },

    mt8: { marginTop: 8 },
    mt16: { marginTop: 16 },
    mb16: { marginBottom: 16 },

    // Layout
    flex1: { flex: 1 },
    row: { flexDirection: 'row' },
    column: { flexDirection: 'column' },
    alignCenter: { alignItems: 'center' },
    justifyBetween: { justifyContent: 'space-between' },

    // Position
    relative: { position: 'relative' },
    absolute: { position: 'absolute' },
    top0: { top: 0 },
    bottom0: { bottom: 0 },

    // Appearance
    bg: { backgroundColor: colors.background },
    bgSurface: { backgroundColor: colors.surface },
    bgPrimary: { backgroundColor: colors.primary },
    bgSecondary: { backgroundColor: colors.secondary },
    roundedLg: { borderRadius: 12 },
    border: { borderWidth: 1, borderColor: colors.border },
  });

export const createGlobalTextStyles = (colors: ThemeColors) =>
  StyleSheet.create({
    textSm: { fontSize: 14 },
    textMd: { fontSize: 16 },
    textLg: { fontSize: 18 },
    textXl: { fontSize: 24 },

    textPrimary: { color: colors.text },
    textSecondary: { color: colors.textSecondary },
    textOnPrimary: { color: colors.white },

    fontBold: { fontWeight: '700' },
    textCenter: { textAlign: 'center' },
  });
