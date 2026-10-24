import React from 'react';
import { Pressable, useWindowDimensions } from 'react-native';
import { usePathname } from 'expo-router';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import {
  AddIcon,
  CalendarIcon,
  ChecklistMinimalisticIcon,
  HomeIcon,
} from '@solar-icons/react-native/linear';

import { useTheme } from '../../context/ThemeContext';
import ModulIconCycle from './ModulIconCycle';

type ModulFabProps = {
  onPress?: () => void;
  /** Con false el icono se detiene (por ejemplo, mientras hay un modal abierto). */
  active?: boolean;
};

const TAB_COUNT = 4;
const FAB_SIZE = 52;
const MENU_MIN_HEIGHT = 58 + 6;
const GAP = 12;

// El primero que se muestra es el de agregar.
const ICONS = [AddIcon, ChecklistMinimalisticIcon, CalendarIcon, HomeIcon];

export default function ModulFab({ onPress, active = true }: ModulFabProps) {
  const { colors, globalStyles } = useTheme();
  const insets = useSafeAreaInsets();
  const { width } = useWindowDimensions();
  const pathname = usePathname();

  if (pathname.startsWith('/tabs/onboarding') || pathname.startsWith('/tabs/auth')) {
    return null;
  }

  return (
    <Pressable
      onPress={onPress}
      accessibilityRole="button"
      accessibilityLabel="Crear"
      style={({ pressed }) => [
        globalStyles.absolute,
        globalStyles.bgPrimary,
        globalStyles.roundedFull,
        globalStyles.alignCenter,
        globalStyles.justifyCenter,
        globalStyles.floating,
        pressed && globalStyles.opacityPressed,
        {
          width: FAB_SIZE,
          height: FAB_SIZE,
          right: width / TAB_COUNT / 2 - FAB_SIZE / 2,
          bottom: MENU_MIN_HEIGHT + Math.max(insets.bottom, 8) + GAP,
        },
      ]}
    >
      <ModulIconCycle icons={ICONS} color={colors.onAccent} active={active} />
    </Pressable>
  );
}
