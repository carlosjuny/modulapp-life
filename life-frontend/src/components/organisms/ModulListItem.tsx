import React from 'react';
import { Pressable } from 'react-native';

import { useTheme } from '../../context/ThemeContext';
import ModulText from '../atoms/ModulText';
import ModulView from '../atoms/ModulView';

type ListItemProps = {
  title: string;
  subtitle?: string;
  left?: React.ReactNode;
  right?: React.ReactNode;
  onPress?: () => void;
  disabled?: boolean;
};

export default function ModulListItem({
                                        title,
                                        subtitle,
                                        left,
                                        right,
                                        onPress,
                                        disabled = false,
                                      }: ListItemProps) {
  const { globalStyles } = useTheme();

  const rowStyle = [
    globalStyles.minH44,
    globalStyles.px16,
    globalStyles.py12,
    globalStyles.row,
    globalStyles.alignCenter,
    globalStyles.gap12,
    globalStyles.borderTop,
  ];

  const content = (
    <>
      {left}
      <ModulView flex1>
        <ModulText>{title}</ModulText>
        {subtitle && <ModulText variant="bodySecondary">{subtitle}</ModulText>}
      </ModulView>
      {right}
    </>
  );

  if (!onPress) {
    return <ModulView style={rowStyle}>{content}</ModulView>;
  }

  return (
    <Pressable
      onPress={onPress}
      disabled={disabled}
      accessibilityRole="button"
      accessibilityLabel={subtitle ? `${title}, ${subtitle}` : title}
      accessibilityState={{ disabled }}
      style={({ pressed }) => [
        ...rowStyle,
        disabled && globalStyles.opacityDisabled,
        pressed && !disabled && globalStyles.opacityPressed,
      ]}
    >
      {content}
    </Pressable>
  );
}
