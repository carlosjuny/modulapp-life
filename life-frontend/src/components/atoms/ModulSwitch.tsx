import React from 'react';
import { Pressable, Switch } from 'react-native';

import { useTheme } from '../../context/ThemeContext';
import ModulText from './ModulText';

type SwitchProps = {
  value: boolean;
  onValueChange: (value: boolean) => void;
  label?: string;
  disabled?: boolean;
};

export default function ModulSwitch({
                                      value,
                                      onValueChange,
                                      label,
                                      disabled = false,
                                    }: SwitchProps) {
  const { colors, globalStyles } = useTheme();

  return (
    <Pressable
      onPress={() => onValueChange(!value)}
      disabled={disabled}
      accessibilityRole="switch"
      accessibilityLabel={label}
      accessibilityState={{ checked: value, disabled }}
      style={[
        globalStyles.minH44,
        globalStyles.minW44,
        globalStyles.row,
        globalStyles.alignCenter,
        globalStyles.justifyBetween,
        globalStyles.gap12,
        disabled && globalStyles.opacityDisabled,
      ]}
    >
      {label && <ModulText style={globalStyles.flex1}>{label}</ModulText>}
      <Switch
        value={value}
        onValueChange={onValueChange}
        disabled={disabled}
        trackColor={{ false: colors.borderStrong, true: colors.primary }}
        thumbColor={colors.tertiary}
      />
    </Pressable>
  );
}
