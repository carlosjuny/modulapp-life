
import React from 'react';
import { Text, Pressable } from 'react-native';
import type { BottomTabBarProps } from 'expo-router/build/react-navigation/bottom-tabs/types';
import { useSafeAreaInsets } from 'react-native-safe-area-context';

import {
  HomeIcon,
  CalendarIcon,
  InboxIcon,
  LayersIcon
} from '@solar-icons/react-native/linear';

import ModulView from '../atoms/ModulView';
import { useTheme } from '../../context/ThemeContext';

export default function ModulMenu({
                                    state,
                                    descriptors,
                                    emitter,
                                    navigateToTab,
                                  }: BottomTabBarProps) {
  const { colors, globalStyles, globalTextStyles } = useTheme();
  const insets = useSafeAreaInsets();

  const tabs = [
    { routeName: 'home' },
    { routeName: 'mailbox' },
    { routeName: 'planer' },
    { routeName: 'moduls' },
  ];

  const getIcon = (routeName: string, color: string) => {
    const props = { size: 26, color };

    switch (routeName) {
      case 'home':
        return <HomeIcon {...props} />;
      case 'mailbox':
        return <InboxIcon {...props} />;
      case 'planer':
        return <CalendarIcon {...props} />;
      case 'moduls':
        return <LayersIcon {...props} />;
      default:
        return null;
    }
  };

  return (
    <ModulView
      row
      alignCenter
      pt6
      borderTop
      bg
      style={{
        borderTopColor: colors.border,
        paddingBottom: Math.max(insets.bottom, 8),
      }}
    >
      {tabs.map((tab) => {
        const routeIndex = state.routes.findIndex(
          (route) => route.name === tab.routeName,
        );

        if (routeIndex === -1) return null;

        const route = state.routes[routeIndex];
        const { options } = descriptors[route.key];
        const isFocused = state.index === routeIndex;

        const iconColor = isFocused
          ? colors.primary
          : colors.textSecondary;

        const onPress = () => {
          const event = emitter.emit({
            type: 'tabPress',
            target: route.key,
            canPreventDefault: true,
          });

          if (!isFocused && !event.defaultPrevented) {
            navigateToTab(route.key);
          }
        };

        const onLongPress = () => {
          emitter.emit({
            type: 'tabLongPress',
            target: route.key,
          });
        };

        return (
          <Pressable
            key={route.key}
            accessibilityRole="tab"
            accessibilityState={{ selected: isFocused }}
            testID={options.tabBarButtonTestID}
            onPress={onPress}
            onLongPress={onLongPress}
            style={[
              globalStyles.flex1,
              globalStyles.alignCenter,
              globalStyles.justifyCenter,
              globalStyles.px2,
              globalStyles.minH58,
              globalStyles.gap4,
            ]}
          >
            <ModulView
              absolute
              top0
              w28
              h3
              roundedFull
              style={{
                backgroundColor: isFocused
                  ? colors.primary
                  : colors.transparent,
              }}
            />

            {getIcon(tab.routeName, iconColor)}

            <Text
              numberOfLines={1}
              style={[
                globalTextStyles.textCenter,
                isFocused ? globalTextStyles.fontBold : {},
                { color: iconColor },
              ]}
            >
            </Text>
          </Pressable>
        );
      })}
    </ModulView>
  );
}
