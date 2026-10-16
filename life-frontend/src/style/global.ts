
import { StyleSheet } from 'react-native';
import type { ThemeColors } from '../theme/colors';

export const createGlobalStyles = (colors: ThemeColors) =>
  StyleSheet.create({
    // Dimensions
    w100: { width: '100%' },
    w50: { width: '50%' },
    h100: { height: 100 },
    hFull: { height: '100%' },

    // Spacing (escala: 4, 8, 12, 16, 24, 32, 48, 64)
    p0: { padding: 0 },
    p4: { padding: 4 },
    p8: { padding: 8 },
    p12: { padding: 12 },
    p16: { padding: 16 },
    p24: { padding: 24 },
    p32: { padding: 32 },
    p48: { padding: 48 },
    p64: { padding: 64 },

    // Margen lateral estándar (16) y vertical
    px16: { paddingHorizontal: 16 },
    px24: { paddingHorizontal: 24 },
    py8: { paddingVertical: 8 },
    py16: { paddingVertical: 16 },

    mt4: { marginTop: 4 },
    mt8: { marginTop: 8 },
    mt12: { marginTop: 12 },
    mt16: { marginTop: 16 },
    mt24: { marginTop: 24 },
    mt32: { marginTop: 32 },
    mt48: { marginTop: 48 },
    mt64: { marginTop: 64 },

    mb4: { marginBottom: 4 },
    mb8: { marginBottom: 8 },
    mb12: { marginBottom: 12 },
    mb16: { marginBottom: 16 },
    mb24: { marginBottom: 24 },
    mb32: { marginBottom: 32 },
    mb48: { marginBottom: 48 },
    mb64: { marginBottom: 64 },

    gap4: { gap: 4 },
    gap8: { gap: 8 },
    gap12: { gap: 12 },
    gap16: { gap: 16 },
    gap24: { gap: 24 },

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

    // Radios: 8 por defecto, 12 en superficies grandes
    rounded: { borderRadius: 8 },
    roundedLg: { borderRadius: 12 },

    // Elevación: borde primero, sombra suave solo en elementos flotantes
    border: { borderWidth: 1, borderColor: colors.border },
    floating: {
      shadowColor: colors.black,
      shadowOffset: { width: 0, height: 4 },
      shadowOpacity: 0.12,
      shadowRadius: 8,
      elevation: 4,
    },
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
