import React from 'react';
import { Text as RNText, type TextProps as RNTextProps } from 'react-native';

import { useTheme } from '../../context/ThemeContext';
import {
  MAX_FONT_SIZE_MULTIPLIER,
  typography,
  type TextVariant,
} from '../../theme/typography';

type TextProps = RNTextProps & {
  variant?: TextVariant;
};

export default function ModulText({
                               variant = 'body',
                               style,
                               maxFontSizeMultiplier = MAX_FONT_SIZE_MULTIPLIER,
                               ...rest
                             }: TextProps) {
  const { colors } = useTheme();

  const color =
    variant === 'bodySecondary' ? colors.textSecondary : colors.text;

  return (
    <RNText
      allowFontScaling
      maxFontSizeMultiplier={maxFontSizeMultiplier}
      style={[typography[variant], { color }, style]}
      {...rest}
    />
  );
}
