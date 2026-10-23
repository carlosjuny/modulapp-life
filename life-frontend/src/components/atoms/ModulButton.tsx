import React from 'react';
import { ActivityIndicator, Pressable, type StyleProp, type ViewStyle } from 'react-native';

import { useTheme } from '../../context/ThemeContext';
import ModulText from './ModulText';

export type ButtonVariant = 'primary' | 'secondary' | 'text' | 'destructive';

type ButtonProps = {
  title: string;
  onPress?: () => void;
  variant?: ButtonVariant;
  disabled?: boolean;
  loading?: boolean;
  fullWidth?: boolean;
  accessibilityLabel?: string;
  style?: StyleProp<ViewStyle>;
};

export default function ModulButton({
                                      title,
                                      onPress,
                                      variant = 'primary',
                                      disabled = false,
                                      loading = false,
                                      fullWidth = false,
                                      accessibilityLabel,
                                      style,
                                    }: ButtonProps) {
  const { colors, globalStyles, globalTextStyles } = useTheme();

  const filled = variant === 'primary' || variant === 'destructive';
  const inactive = disabled || loading;

  const variantStyle = {
    primary: globalStyles.bgPrimary,
    secondary: globalStyles.borderPrimary,
    text: null,
    destructive: globalStyles.bgDanger,
  }[variant];

  return (
    <Pressable
      onPress={onPress}
      disabled={inactive}
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel ?? title}
      accessibilityState={{ disabled: inactive, busy: loading }}
      style={({ pressed }) => [
        globalStyles.minH44,
        globalStyles.minW44,
        globalStyles.px16,
        globalStyles.py8,
        globalStyles.rounded,
        globalStyles.row,
        globalStyles.alignCenter,
        globalStyles.justifyCenter,
        globalStyles.gap8,
        fullWidth ? globalStyles.w100 : globalStyles.selfStart,
        variantStyle,
        disabled && globalStyles.opacityDisabled,
        pressed && !inactive && globalStyles.opacityPressed,
        style,
      ]}
    >
      {loading && <ActivityIndicator color={filled ? colors.onAccent : colors.primary} />}
      <ModulText
        variant="button"
        style={filled ? globalTextStyles.textOnPrimary : globalTextStyles.textBrand}
      >
        {title}
      </ModulText>
    </Pressable>
  );
}
