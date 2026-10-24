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

export type ThemeAccent =
  | 'technology' | 'life' | 'business' | 'services'
  | 'auto' | 'ai' | 'sport';

export const THEME_ACCENTS: ThemeAccent[] = [
  'technology', 'life', 'business', 'services', 'auto', 'ai', 'sport',
];

export const DEFAULT_ACCENT: ThemeAccent = 'life';

// gradient: círculo del selector | light/dark: color plano de la interfaz
export const ACCENT_COLORS: Record<
  ThemeAccent,
  { gradient: [string, string]; light: string; dark: string }
> = {
  technology: { gradient: ['#EF3D5F', '#720C1F'], light: '#720C1F', dark: '#F15A77' },
  life: { gradient: ['#9B57B3', '#364998'], light: '#364998', dark: '#B481C6' },
  business: { gradient: ['#4DFFDB', '#196958'], light: '#196958', dark: '#4DFFDB' },
  services: { gradient: ['#23A7F0', '#095178'], light: '#095178', dark: '#23A7F0' },
  auto: { gradient: ['#F1C40F', '#5C4B00'], light: '#5C4B00', dark: '#F1C40F' },
  ai: { gradient: ['#000000', '#720C1F'], light: '#000000', dark: '#C79EA5' },
  sport: { gradient: ['#FA6A0D', '#161717'], light: '#161717', dark: '#FA6A0D' },
};

export const lightHighContrastColors: ThemeColors = {
  ...lightColors,
  tertiary: '#FFFFFF',
  background: '#FFFFFF',
  surface: '#F1F5F9',
  text: '#000000',
  textSecondary: '#1E293B',
  border: '#000000',
  borderStrong: '#000000',
  success: '#14532D',
  warning: '#78350F',
  danger: '#7F1D1D',
  info: '#0C4A6E',
};

export const darkHighContrastColors: ThemeColors = {
  ...darkColors,
  tertiary: '#0F172A',
  background: '#000000',
  surface: '#0F172A',
  text: '#FFFFFF',
  textSecondary: '#E2E8F0',
  border: '#FFFFFF',
  borderStrong: '#FFFFFF',
  success: '#86EFAC',
  warning: '#FDE68A',
  danger: '#FCA5A5',
  info: '#7DD3FC',
  onAccent: '#000000',
};

const mix = (from: string, to: string, amount: number) => {
  const a = [1, 3, 5].map((i) => parseInt(from.slice(i, i + 2), 16));
  const b = [1, 3, 5].map((i) => parseInt(to.slice(i, i + 2), 16));
  const hex = a.map((v, i) =>
    Math.round(v + (b[i] - v) * amount).toString(16).padStart(2, '0'),
  );
  return `#${hex.join('').toUpperCase()}`;
};

export function buildPalette(
  isDark: boolean,
  accent: ThemeAccent,
  highContrast: boolean,
): ThemeColors {
  const base = highContrast
    ? isDark ? darkHighContrastColors : lightHighContrastColors
    : isDark ? darkColors : lightColors;

  const color = isDark ? ACCENT_COLORS[accent].dark : ACCENT_COLORS[accent].light;
  const primary = highContrast
    ? isDark ? mix(color, '#FFFFFF', 0.5) : mix(color, '#000000', 0.4)
    : color;

  return { ...base, primary, secondary: primary };
}
