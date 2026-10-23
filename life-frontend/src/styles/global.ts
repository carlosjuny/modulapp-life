
import { StyleSheet } from 'react-native';
import type { ThemeColors } from '../theme/colors';

// Tokens de layout de pantalla (los usa ModulLayout)
export const layout = {
  maxContentWidth: 720,
  screenMargin: 16,
  screenPaddingY: 16,
} as const;

export const createGlobalStyles = (colors: ThemeColors) =>
  StyleSheet.create({
    // Dimensions
    w100: { width: '100%' },
    w50: { width: '50%' },
    h100: { height: 100 },
    hFull: { height: '100%' },

    minH58: { minHeight: 58 },
    w28: { width: 28 },
    h3: { height: 3 },

    minH44: { minHeight: 44 },
    minW44: { minWidth: 44 },

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

    pt6: { paddingTop: 6 },

    // Margen lateral estándar (16) y vertical
    px2: { paddingHorizontal: 2 },
    py4: { paddingVertical: 4 },
    py12: { paddingVertical: 12 },
    px12: { paddingHorizontal: 12 },
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

    // Fondos
    bgTertiary: { backgroundColor: colors.tertiary },
    bgDanger: { backgroundColor: colors.danger },

    // Bordes de estado
    borderPrimary: { borderWidth: 1, borderColor: colors.primary },
    borderFocus: { borderWidth: 2, borderColor: colors.primary },
    borderError: { borderWidth: 2, borderColor: colors.danger },
    roundedTopLg: { borderTopLeftRadius: 12, borderTopRightRadius: 12 },

    // Estados
    opacityDisabled: { opacity: 0.4 },
    opacityPressed: { opacity: 0.85 },
    backdrop: { backgroundColor: colors.black, opacity: 0.5 },

    // Layout
    flex1: { flex: 1 },
    row: { flexDirection: 'row' },
    column: { flexDirection: 'column' },
    alignCenter: { alignItems: 'center' },
    justifyBetween: { justifyContent: 'space-between' },
    justifyCenter: { justifyContent: 'center' },
    selfStart: { alignSelf: 'flex-start' },
    justifyEnd: { justifyContent: 'flex-end' },
    absoluteFill: { position: 'absolute', top: 0, bottom: 0, left: 0, right: 0 },

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
    roundedFull: { borderRadius: 999 },

    // Elevación: borde primero, sombra suave solo en elementos flotantes
    border: { borderWidth: 1, borderColor: colors.border },
    floating: {
      shadowColor: colors.black,
      shadowOffset: { width: 0, height: 4 },
      shadowOpacity: 0.12,
      shadowRadius: 8,
      elevation: 4,
    },

    // Borders
    borderTop: {
      borderTopWidth: StyleSheet.hairlineWidth,
      borderTopColor: colors.border,
    },
  });

export const createGlobalTextStyles = (colors: ThemeColors) =>
  StyleSheet.create({
    // Los tamaños viven en las variantes del componente Text (theme/typography.ts)
    textPrimary: { color: colors.text },
    textSecondary: { color: colors.textSecondary },
    textOnPrimary: { color: colors.white },
    textBrand: { color: colors.primary }, // NUEVO
    textDanger: { color: colors.danger },

    fontBold: { fontWeight: '700' },
    textCenter: { textAlign: 'center' },
  });
