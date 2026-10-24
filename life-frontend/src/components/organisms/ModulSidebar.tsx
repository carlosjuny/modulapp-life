import React, { useEffect, useState } from 'react';
import { ActivityIndicator, Animated, Modal, Pressable, useWindowDimensions } from 'react-native';
import { usePathname, useRouter } from 'expo-router';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import {
  SettingsIcon,
  UserCircleIcon,
  LogoutIcon,
  PanelLeftCloseIcon,
  PaletteIcon,
} from '@solar-icons/react-native/linear';

import { useReduceMotion } from '../../hooks/useReduceMotion';
import { useTheme } from '../../context/ThemeContext';
import { useAuth } from '../../context/AuthContext';
import { strings } from '../../constants/strings';
import ModulView from '../atoms/ModulView';
import ModulText from '../atoms/ModulText';
import ModulIcon from '../atoms/ModulIcon';
import ModulAvatar from '../atoms/ModulAvatar';
import ModulThemeModal from './ModulThemeModal';

const PROFILE_HREF = '/tabs/profile/profile';

const ITEMS = [
  { label: strings.sidebar.settings, icon: SettingsIcon, href: '/tabs/settings/settings' },
];

type Props = { visible: boolean; onClose: () => void };

export default function ModulSidebar({ visible, onClose }: Props) {
  const { colors } = useTheme();
  const { user, status, signOut } = useAuth();
  const reduceMotion = useReduceMotion();
  const router = useRouter();
  const pathname = usePathname();
  const insets = useSafeAreaInsets();
  const { width: screenWidth } = useWindowDimensions();
  const panelWidth = Math.min(screenWidth * 0.78, 320);

  const [progress] = useState(() => new Animated.Value(0));
  const [mounted, setMounted] = useState(visible);
  const [themeOpen, setThemeOpen] = useState(false);

  if (visible && !mounted) setMounted(true);

  useEffect(() => {
    Animated.timing(progress, {
      toValue: visible ? 1 : 0,
      duration: reduceMotion ? 0 : visible ? 250 : 200,
      useNativeDriver: true,
    }).start(({ finished }) => {
      if (finished && !visible) setMounted(false);
    });
  }, [visible, progress, reduceMotion]);

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

  const openTheme = () => {
    onClose();
    // Espera a que el sidebar termine de cerrarse antes de abrir el modal
    setTimeout(() => setThemeOpen(true), reduceMotion ? 0 : 300);
  };

  const handleSignOut = async () => {
    onClose();
    await signOut();
    router.replace('/');
  };

  const profileName = user?.name ?? strings.sidebar.profileFallbackName;
  const profileActive = pathname === PROFILE_HREF;

  return (
    <>
      <Modal
      visible={mounted}
      transparent
      animationType="none"
      onRequestClose={onClose}
    >
      <ModulView flex1 row>
        {/* Panel a la izquierda */}
        <Animated.View
          accessibilityViewIsModal
          style={{ width: panelWidth, transform: [{ translateX }] }}
        >
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
              <ModulText variant="sectionTitle">{strings.sidebar.title}</ModulText>
              <Pressable
                onPress={onClose}
                accessibilityRole="button"
                accessibilityLabel={strings.sidebar.close}
              >
                <ModulView p8 roundedLg>
                  <ModulIcon icon={PanelLeftCloseIcon} color={colors.text} />
                </ModulView>
              </Pressable>
            </ModulView>

            {/* Perfil */}
            <Pressable
              onPress={() => go(PROFILE_HREF)}
              accessibilityRole="button"
              accessibilityLabel={strings.sidebar.profileLabel(profileName)}
              accessibilityState={{ selected: profileActive }}
            >
              <ModulView row alignCenter gap12 mb24 p8 roundedLg bgSurface={profileActive}>
                {status === 'loading' ? (
                  <>
                    <ModulView alignCenter justifyCenter style={{ width: 48, height: 48 }}>
                      <ActivityIndicator color={colors.primary} />
                    </ModulView>
                    <ModulView flex1>
                      <ModulText variant="bodySecondary">
                        {strings.sidebar.profileLoading}
                      </ModulText>
                    </ModulView>
                  </>
                ) : (
                  <>
                    {user ? (
                      <ModulAvatar name={user.name} uri={user.photoUri} size="lg" />
                    ) : (
                      <ModulView
                        bgSurface
                        roundedFull
                        alignCenter
                        justifyCenter
                        style={{ width: 56, height: 56 }}
                      >
                        <ModulIcon icon={UserCircleIcon} color={colors.textSecondary} />
                      </ModulView>
                    )}
                    <ModulView flex1>
                      <ModulText numberOfLines={1}>{profileName}</ModulText>
                      <ModulText variant="bodySecondary" numberOfLines={1}>
                        {user ? user.email : strings.sidebar.profileFallbackHint}
                      </ModulText>
                    </ModulView>
                  </>
                )}
              </ModulView>
            </Pressable>

            {/* Entradas */}
            {ITEMS.map(({ label, icon, href }) => {
              const active = pathname.startsWith('/tabs/settings');
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

            {/* Tema */}
            <Pressable
              onPress={openTheme}
              accessibilityRole="button"
              accessibilityLabel={strings.sidebar.theme}
            >
              <ModulView row alignCenter gap12 p12 roundedLg>
                <ModulIcon icon={PaletteIcon} color={colors.textSecondary} />
                <ModulText>{strings.sidebar.theme}</ModulText>
              </ModulView>
            </Pressable>

            {/* Cerrar sesión */}
            <Pressable
              onPress={handleSignOut}
              accessibilityRole="button"
              accessibilityLabel={strings.sidebar.signOut}
              style={{ marginTop: 'auto' }}
            >
              <ModulView row alignCenter gap12 p12>
                <ModulIcon icon={LogoutIcon} color={colors.danger} />
                <ModulText style={{ color: colors.danger }}>
                  {strings.sidebar.signOut}
                </ModulText>
              </ModulView>
            </Pressable>
          </ModulView>
        </Animated.View>

        {/* Fondo para cerrar */}
        <Animated.View
          style={{ flex: 1, backgroundColor: colors.black, opacity: overlayOpacity }}
        >
          <Pressable
            style={{ flex: 1 }}
            onPress={onClose}
            accessibilityRole="button"
            accessibilityLabel={strings.sidebar.close}
          />
        </Animated.View>
      </ModulView>
    </Modal>
      <ModulThemeModal visible={themeOpen} onClose={() => setThemeOpen(false)} />
    </>
  );
}
