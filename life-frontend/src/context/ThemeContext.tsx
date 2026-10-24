import React, {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from 'react';
import { useColorScheme } from 'react-native';
import AsyncStorage from '@react-native-async-storage/async-storage';

import {
  buildPalette,
  THEME_ACCENTS,
  DEFAULT_ACCENT,
  type ThemeAccent,
  type ThemeColors,
  type ThemePreference,
} from '../theme/colors';

import {
  createGlobalStyles,
  createGlobalTextStyles,
} from '../styles/global';

const THEME_STORAGE_KEY = '@modulapp/theme';
const ACCENT_STORAGE_KEY = '@modulapp/theme-accent';
const CONTRAST_STORAGE_KEY = '@modulapp/high-contrast';

type ThemeContextValue = {
  preference: ThemePreference;
  setPreference: (theme: ThemePreference) => void;
  accent: ThemeAccent;
  setAccent: (accent: ThemeAccent) => void;
  highContrast: boolean;
  setHighContrast: (value: boolean) => void;
  isDark: boolean;
  colors: ThemeColors;
  globalStyles: ReturnType<typeof createGlobalStyles>;
  globalTextStyles: ReturnType<typeof createGlobalTextStyles>;
};

const ThemeContext = createContext<ThemeContextValue | undefined>(undefined);

const isPreference = (v: unknown): v is ThemePreference =>
  v === 'light' || v === 'dark' || v === 'system';

const isAccent = (v: unknown): v is ThemeAccent =>
  THEME_ACCENTS.includes(v as ThemeAccent);

export function ThemeProvider({ children }: { children: React.ReactNode }) {
  const systemTheme = useColorScheme();

  const [preference, setPreferenceState] = useState<ThemePreference>('light');
  const [accent, setAccentState] = useState<ThemeAccent>(DEFAULT_ACCENT);
  const [highContrast, setHighContrastState] = useState(false);
  const [loaded, setLoaded] = useState(false);

  useEffect(() => {
    async function loadTheme() {
      try {
        const saved = await AsyncStorage.multiGet([
          THEME_STORAGE_KEY,
          ACCENT_STORAGE_KEY,
          CONTRAST_STORAGE_KEY,
        ]);
        const [theme, savedAccent, contrast] = saved.map(([, value]) => value);

        if (isPreference(theme)) setPreferenceState(theme);
        if (isAccent(savedAccent)) setAccentState(savedAccent);
        if (contrast === 'true') setHighContrastState(true);
      } catch (error) {
        console.warn('No se pudo cargar el tema guardado.', error);
      } finally {
        setLoaded(true);
      }
    }

    loadTheme();
  }, []);

  const save = (key: string, value: string) => {
    AsyncStorage.setItem(key, value).catch((error) => {
      console.warn('No se pudo guardar la preferencia de tema.', error);
    });
  };

  const setPreference = useCallback((theme: ThemePreference) => {
    setPreferenceState(theme);
    save(THEME_STORAGE_KEY, theme);
  }, []);

  const setAccent = useCallback((value: ThemeAccent) => {
    setAccentState(value);
    save(ACCENT_STORAGE_KEY, value);
  }, []);

  const setHighContrast = useCallback((value: boolean) => {
    setHighContrastState(value);
    save(CONTRAST_STORAGE_KEY, String(value));
  }, []);

  const isDark =
    preference === 'system'
      ? systemTheme === 'dark'
      : preference === 'dark';

  const colors = useMemo(
    () => buildPalette(isDark, accent, highContrast),
    [isDark, accent, highContrast],
  );

  const globalStyles = useMemo(() => createGlobalStyles(colors), [colors]);
  const globalTextStyles = useMemo(() => createGlobalTextStyles(colors), [colors]);

  const value = useMemo(
    () => ({
      preference,
      setPreference,
      accent,
      setAccent,
      highContrast,
      setHighContrast,
      isDark,
      colors,
      globalStyles,
      globalTextStyles,
    }),
    [
      preference, setPreference, accent, setAccent, highContrast,
      setHighContrast, isDark, colors, globalStyles, globalTextStyles,
    ],
  );

  // Evita mostrar el tema predeterminado antes de recuperar la preferencia.
  if (!loaded) {
    return null;
  }

  return (
    <ThemeContext.Provider value={value}>
      {children}
    </ThemeContext.Provider>
  );
}

export function useTheme() {
  const context = useContext(ThemeContext);

  if (!context) {
    throw new Error('useTheme debe utilizarse dentro de ThemeProvider.');
  }

  return context;
}
