
import type { ViewProps, ViewStyle, StyleProp } from 'react-native';
import type { createGlobalStyles } from './global';

export type GlobalStyleProps = {
  [K in keyof ReturnType<typeof createGlobalStyles>]?: boolean;
};

export type GlobalProps = ViewProps &
  GlobalStyleProps & {
  style?: StyleProp<ViewStyle>;
};
