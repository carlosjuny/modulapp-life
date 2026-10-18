
import React, {
  createContext,
  useContext,
  useEffect,
  useMemo,
  useState,
} from 'react';
import { useColorScheme } from 'react-native';
import AsyncStorage from '@react-native-async-storage/async-storage';

import {
  lightColors,
  darkColors,
  type ThemePreference,
} from '../theme/colors';

import {
  createGlobalStyles,
  createGlobalTextStyles,
} from '../styles/global';

const THEME_STORAGE_KEY = '@modulapp/theme';

type ThemeContextValue = {
  preference: ThemePreference;
  setPreference: (theme: ThemePreference) => void;
  isDark: boolean;
  colors: typeof lightColors;
  globalStyles: ReturnType<typeof createGlobalStyles>;
  globalTextStyles: ReturnType<typeof createGlobalTextStyles>;
};

const ThemeContext = createContext<ThemeContextValue | undefined>(undefined);

export function ThemeProvider({
                                children,
                              }: {
  children: React.ReactNode;
}) {
  const systemTheme = useColorScheme();

  const [preference, setPreferenceState] =
    useState<ThemePreference>('light');

  const [loaded, setLoaded] = useState(false);

  useEffect(() => {
    async function loadTheme() {
      try {
        const savedTheme = await AsyncStorage.getItem(THEME_STORAGE_KEY);

        if (
          savedTheme === 'light' ||
          savedTheme === 'dark' ||
          savedTheme === 'system'
        ) {
          setPreferenceState(savedTheme);
        }
      } catch (error) {
        console.warn('No se pudo cargar el tema guardado.', error);
      } finally {
        setLoaded(true);
      }
    }

    loadTheme();
  }, []);

  const setPreference = (theme: ThemePreference) => {
    setPreferenceState(theme);

    AsyncStorage.setItem(THEME_STORAGE_KEY, theme).catch((error) => {
      console.warn('No se pudo guardar el tema.', error);
    });
  };


  const isDark =
    preference === 'system'
      ? systemTheme === 'dark'
      : preference === 'dark';

  const colors = isDark ? darkColors : lightColors;

  const globalStyles = useMemo(
    () => createGlobalStyles(colors),
    [colors],
  );

  const globalTextStyles = useMemo(
    () => createGlobalTextStyles(colors),
    [colors],
  );

  const value = useMemo(
    () => ({
      preference,
      setPreference,
      isDark,
      colors,
      globalStyles,
      globalTextStyles,
    }),
    [preference, isDark, colors, globalStyles, globalTextStyles],
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
    throw new Error(
      'useTheme debe utilizarse dentro de ThemeProvider.',
    );
  }

  return context;
}
