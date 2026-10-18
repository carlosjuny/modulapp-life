
import React from 'react';
import { Pressable, View } from 'react-native';
import { useRouter } from 'expo-router';

import {
  AltArrowLeftIcon,
  BellIcon,
  HamburgerMenuIcon,
} from '@solar-icons/react-native/linear';

import { useTheme } from '../../context/ThemeContext';
import ModulView from '../atoms/ModulView';
import ModulIcon from '../atoms/ModulIcon';
import ModulText from '../atoms/ModulText';

type ModulHeaderProps = {
  title: string;
  subtitle?: string;

  leftAction?: 'menu' | 'back' | 'none';
  showNotifications?: boolean;

  onMenuPress?: () => void;
  onBackPress?: () => void;
  onNotificationsPress?: () => void;

  leftIcon?: React.ComponentProps<typeof ModulIcon>['icon'];
  rightIcon?: React.ComponentProps<typeof ModulIcon>['icon'];
  onRightPress?: () => void;

  centered?: boolean;
  showBorder?: boolean;

  background?: 'bg' | 'bgSurface';
};

export default function ModulHeader({
                                      title,
                                      subtitle,
                                      leftAction = 'menu',
                                      showNotifications = true,
                                      onMenuPress,
                                      onBackPress,
                                      onNotificationsPress,
                                      leftIcon,
                                      rightIcon,
                                      onRightPress,
                                      centered = false,
                                      showBorder = true,
                                      background = 'bgSurface',
                                    }: ModulHeaderProps) {
  const router = useRouter();
  const { colors, globalStyles, globalTextStyles } = useTheme();

  const handleLeftPress = () => {
    if (leftAction === 'back') {
      if (onBackPress) {
        onBackPress();
      } else if (router.canGoBack()) {
        router.back();
      }

      return;
    }

    if (leftAction === 'menu') {
      onMenuPress?.();
    }
  };

  const iconButtonStyle = [
    globalStyles.alignCenter,
    globalStyles.justifyCenter,
    globalStyles.roundedLg,
  ];

  return (
    <ModulView
      row
      alignCenter
      p16
      bg
      style={{
        borderBottomColor: colors.border,
      }}
    >
      {/* Lado izquierdo: conserva siempre el espacio */}
      <View style={{ width: 44, flexShrink: 0 }}>
        {leftAction !== 'none' && (
          <Pressable
            onPress={handleLeftPress}
            accessibilityRole="button"
            accessibilityLabel={
              leftAction === 'back' ? 'Volver' : 'Abrir menú'
            }
            style={iconButtonStyle}
          >
            <ModulIcon
              icon={
                leftIcon ??
                (leftAction === 'back'
                  ? AltArrowLeftIcon
                  : HamburgerMenuIcon)
              }
              color={colors.text}
            />
          </Pressable>
        )}
      </View>

      {/* Centro: ocupa el espacio disponible */}
      <View
        style={{
          flex: 1,
          minWidth: 0,
          alignItems: centered ? 'center' : 'flex-start',
          justifyContent: 'center',
        }}
      >
        <ModulText
          numberOfLines={1}
          style={[
            globalTextStyles.textPrimary,
          ]}
        >
          {title}
        </ModulText>

        {subtitle ? (
          <ModulText
            numberOfLines={1}
            style={[
              { color: colors.textSecondary },
            ]}
          >
            {subtitle}
          </ModulText>
        ) : null}
      </View>

      {/* Lado derecho: conserva el mismo espacio */}
      <View
        style={{
          width: 44,
          flexShrink: 0,
          alignItems: 'center',
        }}
      >
        {rightIcon ? (
          <Pressable
            onPress={onRightPress}
            accessibilityRole="button"
            accessibilityLabel="Acción adicional"
            style={iconButtonStyle}
          >
            <ModulIcon
              icon={rightIcon}
              color={colors.text}
            />
          </Pressable>
        ) : showNotifications ? (
          <Pressable
            onPress={onNotificationsPress}
            accessibilityRole="button"
            accessibilityLabel="Notificaciones"
            style={iconButtonStyle}
          >
            <ModulIcon
              icon={BellIcon}
              color={colors.text}
            />
          </Pressable>
        ) : null}
      </View>
    </ModulView>
  );
}
