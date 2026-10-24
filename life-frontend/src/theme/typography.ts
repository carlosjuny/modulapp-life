import type { TextStyle } from 'react-native';

export const MIN_FONT_SIZE = 14;

export const MAX_FONT_SIZE_MULTIPLIER = 1.5;

export type TextVariant =
  | 'screenTitle'
  | 'sectionTitle'
  | 'headerTitle'
  | 'body'
  | 'bodySecondary'
  | 'label'
  | 'button';

type VariantStyle = Required<Pick<TextStyle, 'fontSize' | 'lineHeight'>> &
  Pick<TextStyle, 'fontWeight'>;

export const typography: Record<TextVariant, VariantStyle> = {
  screenTitle: { fontSize: 24, lineHeight: 32, fontWeight: '700' },
  sectionTitle: { fontSize: 18, lineHeight: 27, fontWeight: '600' },
  body: { fontSize: 16, lineHeight: 24, fontWeight: '400' },
  bodySecondary: { fontSize: 14, lineHeight: 21, fontWeight: '400' },
  label: { fontSize: 14, lineHeight: 21, fontWeight: '500' },
  button: { fontSize: 16, lineHeight: 24, fontWeight: '600' },
  headerTitle: { fontSize: 22, fontWeight: '700', lineHeight: 32 },
};
