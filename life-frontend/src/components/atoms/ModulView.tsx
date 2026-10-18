
import React from 'react';
import { View } from 'react-native';

import { useTheme } from '../../context/ThemeContext';
import { createGlobalStyles } from '../../styles/global';
import type { GlobalProps } from '../../styles/globalProps';

export type { GlobalProps } from '../../styles/globalProps';

export default function ModulView({
                                    children,
                                    style,
                                    ...props
                                  }: GlobalProps) {
  const { colors } = useTheme();
  const styles = createGlobalStyles(colors);

  const appliedStyles = Object.keys(styles)
    .filter((key) => props[key as keyof typeof props] === true)
    .map((key) => styles[key as keyof typeof styles]);

  const viewProps = Object.fromEntries(
    Object.entries(props).filter(([key]) => !(key in styles)),
  );

  return (
    <View {...viewProps} style={[...appliedStyles, style]}>
      {children}
    </View>
  );
}
