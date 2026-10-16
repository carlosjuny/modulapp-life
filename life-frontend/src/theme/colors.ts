
export const lightColors = {
  primary: '#364998',
  secondary: '#9B57B3',
  tertiary: '#FFFCFA',

  background: '#F8FAFC',
  surface: '#e2e2e2',
  text: '#0F172A',
  textSecondary: '#64748B',
  border: '#E2E8F0',

  success: '#16A34A',
  warning: '#F59E0B',
  danger: '#DC2626',
  info: '#0284C7',

  white: '#FFFFFF',
  black: '#000000',
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

  success: '#4ADE80',
  warning: '#FBBF24',
  danger: '#F87171',
  info: '#38BDF8',

  white: '#FFFFFF',
  black: '#000000',
  transparent: 'transparent',
};

export type ThemeColors = typeof lightColors;
export type ThemePreference = 'light' | 'dark' | 'system';
