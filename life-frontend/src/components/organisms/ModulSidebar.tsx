import React, { useEffect, useState } from 'react';
import { Animated, Modal, Pressable, useWindowDimensions } from 'react-native';
import { usePathname, useRouter } from 'expo-router';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import {
  SettingsIcon,
  UserCircleIcon,
  LogoutIcon,
  PanelLeftCloseIcon,
} from '@solar-icons/react-native/linear';

import { useTheme } from '../../context/ThemeContext';
import ModulView from '../atoms/ModulView';
import ModulText from '../atoms/ModulText';
import ModulIcon from '../atoms/ModulIcon';

// Usuario demo (sin Auth ni API)
const DEMO_USER = {
  name: 'Usuario Demo',
  email: 'demo@modulapp.com',
};

const ITEMS = [
  { label: 'Ajustes de la cuenta', icon: UserCircleIcon, href: '/tabs/account' },
  { label: 'Configuración de la app', icon: SettingsIcon, href: '/tabs/diagnostico' },
];

type Props = { visible: boolean; onClose: () => void };

export default function ModulSidebar({ visible, onClose }: Props) {
  const { colors } = useTheme();
  const router = useRouter();
  const pathname = usePathname();
  const insets = useSafeAreaInsets();
  const { width: screenWidth } = useWindowDimensions();
  const panelWidth = Math.min(screenWidth * 0.78, 320);

  const [progress] = useState(() => new Animated.Value(0));
  const [mounted, setMounted] = useState(visible);

  if (visible && !mounted) setMounted(true);

  useEffect(() => {
    if (visible) {
      Animated.timing(progress, {
        toValue: 1,
        duration: 250,
        useNativeDriver: true,
      }).start();
    } else {
      Animated.timing(progress, {
        toValue: 0,
        duration: 200,
        useNativeDriver: true,
      }).start(({ finished }) => {
        if (finished) setMounted(false);
      });
    }
  }, [visible, progress]);

  // Entra y sale por el lado izquierdo
  const translateX = progress.interpolate({
    inputRange: [0, 1],
    outputRange: [-panelWidth, 0],
  });
  const overlayOpacity = progress.interpolate({
    inputRange: [0, 1],
    outputRange: [0, 0.4],
  });

  const go = (href: string) => {
    onClose();
    router.push(href as never);
  };

  return (
    <Modal
      visible={mounted}
      transparent
      animationType="none"
      onRequestClose={onClose}
    >
      <ModulView flex1 row>
        {/* Panel a la izquierda */}
        <Animated.View style={{ width: panelWidth, transform: [{ translateX }] }}>
          <ModulView
            flex1
            bg
            px16
            style={{
              paddingTop: insets.top + 16,
              paddingBottom: insets.bottom + 16,
            }}
          >
            {/* Cerrar sidebar */}
            <ModulView row justifyBetween alignCenter mb16>
              <ModulText variant="sectionTitle">Menú</ModulText>
              <Pressable
                onPress={onClose}
                accessibilityRole="button"
                accessibilityLabel="Cerrar menú"
              >
                <ModulView p8 roundedLg>
                  <ModulIcon icon={PanelLeftCloseIcon} color={colors.text} />
                </ModulView>
              </Pressable>
            </ModulView>

            {/* Perfil */}
            <Pressable
              onPress={() => go('/tabs/profile')}
              accessibilityRole="button"
              accessibilityLabel={`Perfil de ${DEMO_USER.name}`}
            >
              <ModulView row alignCenter gap12 mb24>
                <ModulView
                  bgSurface
                  roundedFull
                  alignCenter
                  justifyCenter
                  style={{ width: 48, height: 48 }}
                >
                  <ModulIcon icon={UserCircleIcon} color={colors.textSecondary} />
                </ModulView>
                <ModulView flex1>
                  <ModulText numberOfLines={1}>{DEMO_USER.name}</ModulText>
                  <ModulText variant="bodySecondary" numberOfLines={1}>
                    {DEMO_USER.email}
                  </ModulText>
                </ModulView>
              </ModulView>
            </Pressable>

            {/* Entradas */}
            {ITEMS.map(({ label, icon, href }) => {
              const active = pathname === href;
              return (
                <Pressable
                  key={href}
                  onPress={() => go(href)}
                  accessibilityRole="button"
                  accessibilityLabel={label}
                  accessibilityState={{ selected: active }}
                >
                  <ModulView row alignCenter gap12 p12 roundedLg bgSurface={active}>
                    <ModulIcon
                      icon={icon}
                      color={active ? colors.primary : colors.textSecondary}
                    />
                    <ModulText style={{ color: active ? colors.primary : colors.text }}>
                      {label}
                    </ModulText>
                  </ModulView>
                </Pressable>
              );
            })}

            {/* Cerrar sesión */}
            <Pressable
              onPress={() => {
                onClose();
                router.replace('/');
              }}
              accessibilityRole="button"
              accessibilityLabel="Cerrar sesión"
              style={{ marginTop: 'auto' }}
            >
              <ModulView row alignCenter gap12 p12>
                <ModulIcon icon={LogoutIcon} color={colors.danger} />
                <ModulText style={{ color: colors.danger }}>Cerrar sesión</ModulText>
              </ModulView>
            </Pressable>
          </ModulView>
        </Animated.View>

        {/* Fondo para cerrar */}
        <Animated.View
          style={{ flex: 1, backgroundColor: '#000', opacity: overlayOpacity }}
        >
          <Pressable
            style={{ flex: 1 }}
            onPress={onClose}
            accessibilityLabel="Cerrar menú"
          />
        </Animated.View>
      </ModulView>
    </Modal>
  );
}
