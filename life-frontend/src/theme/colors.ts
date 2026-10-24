export const lightColors = {
  primary: '#364998',
  secondary: '#7E3F99',
  tertiary: '#FFFCFA',

  background: '#F8FAFC',
  surface: '#EEF2F6',
  text: '#0F172A',
  textSecondary: '#475569',
  border: '#E2E8F0',
  borderStrong: '#64748B',

  success: '#15803D',
  warning: '#B45309',
  danger: '#B91C1C',
  info: '#0369A1',

  white: '#FFFFFF',
  black: '#000000',
  onAccent: '#FFFFFF',
  transparent: 'transparent',
};

export const darkColors = {
  primary: '#8B9DF0',
  secondary: '#C58ADB',
  tertiary: '#1E293B',

  background: '#0F172A',
  surface: '#1E293B',
  text: '#F1F5F9',
  textSecondary: '#94A3B8',
  border: '#334155',
  borderStrong: '#64748B',

  success: '#4ADE80',
  warning: '#FBBF24',
  danger: '#F87171',
  info: '#38BDF8',

  white: '#FFFFFF',
  black: '#000000',
  onAccent: '#0F172A',
  transparent: 'transparent',
};

export type ThemeColors = typeof lightColors;
export type ThemePreference = 'light' | 'dark' | 'system';
