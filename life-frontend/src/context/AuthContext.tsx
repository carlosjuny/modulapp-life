import React, { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import AsyncStorage from '@react-native-async-storage/async-storage';

const SESSION_KEY = '@modulapp/session';

export type AuthUser = { name: string; email: string; photoUri?: string };
type Status = 'loading' | 'ready' | 'error';

// Sesión de demostración hasta que exista autenticación real (LIF-87)
const DEMO_USER: AuthUser = { name: 'Usuario Demo', email: 'demo@modulapp.com' };

type AuthContextValue = {
  user: AuthUser | null;
  status: Status;
  signOut: () => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [status, setStatus] = useState<Status>('loading');

  useEffect(() => {
    let active = true;

    (async () => {
      try {
        const saved = await AsyncStorage.getItem(SESSION_KEY);
        const value: AuthUser = saved ? JSON.parse(saved) : DEMO_USER;
        if (!saved) await AsyncStorage.setItem(SESSION_KEY, JSON.stringify(value));
        if (active) {
          setUser(value);
          setStatus('ready');
        }
      } catch {
        if (active) setStatus('error');
      }
    })();

    return () => {
      active = false;
    };
  }, []);

  const signOut = useCallback(async () => {
    try {
      await AsyncStorage.removeItem(SESSION_KEY);
    } catch {
      // el cierre local continúa aunque falle el almacenamiento
    }
    setUser(null);
    setStatus('ready');
  }, []);

  const value = useMemo(() => ({ user, status, signOut }), [user, status, signOut]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth debe usarse dentro de AuthProvider');
  return ctx;
}
