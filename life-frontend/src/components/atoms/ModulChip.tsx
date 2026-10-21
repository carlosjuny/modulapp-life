import React from 'react';
import { Pressable } from 'react-native';

import { useTheme } from '../../context/ThemeContext';
import ModulText from './ModulText';
import ModulView from './ModulView';

type ChipProps = {
  label: string;
  selected?: boolean;
  disabled?: boolean;
  onPress?: () => void;
};

export default function ModulChip({
                                    label,
                                    selected = false,
                                    disabled = false,
                                    onPress,
                                  }: ChipProps) {
  const { globalStyles, globalTextStyles } = useTheme();

  const textStyle = selected ? globalTextStyles.textOnPrimary : globalTextStyles.textPrimary;

  const chipStyle = [
    globalStyles.px12,
    globalStyles.py4,
    globalStyles.roundedFull,
    globalStyles.selfStart,
    globalStyles.alignCenter,
    globalStyles.justifyCenter,
    selected ? globalStyles.bgPrimary : globalStyles.border,
  ];

  if (!onPress) {
    return (
      <ModulView style={chipStyle}>
        <ModulText variant="label" style={textStyle}>{label}</ModulText>
      </ModulView>
    );
  }

  return (
    <Pressable
      onPress={onPress}
      disabled={disabled}
      accessibilityRole="button"
      accessibilityLabel={label}
      accessibilityState={{ selected, disabled }}
      style={({ pressed }) => [
        ...chipStyle,
        globalStyles.minH44,
        globalStyles.minW44,
        disabled && globalStyles.opacityDisabled,
        pressed && !disabled && globalStyles.opacityPressed,
      ]}
    >
      <ModulText variant="label" style={textStyle}>{label}</ModulText>
    </Pressable>
  );
}
