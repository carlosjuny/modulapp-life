
import React from 'react';
import type { ComponentProps } from 'react';

import { HomeIcon } from '@solar-icons/react-native/linear';
import { useTheme } from '../../context/ThemeContext';

type AppIconProps = {
  icon: React.ComponentType<ComponentProps<typeof HomeIcon>>;
  color?: string;
};

export default function AppIcon({
                                  icon: Icon,
                                  color,
                                }: AppIconProps) {
  const { colors } = useTheme();

  return (
    <Icon
      size={22}
      color={color ?? colors.text}
    />
  );
}
