import React from 'react';
import { KeyboardAvoidingView, Platform, ScrollView, View } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';

import { useTheme } from '../../context/ThemeContext';
import { layout } from '../../styles/global';

type ModulLayoutProps = {
  children: React.ReactNode;
  /** Contenido con scroll vertical. Por defecto, sin scroll. */
  scroll?: boolean;
  /** Aplica el margen lateral estándar de pantalla. Por defecto, sin margen. */
  margins?: boolean;
};

export default function ModulLayout({
                                      children,
                                      scroll = false,
                                      margins = false,
                                    }: ModulLayoutProps) {
  const { colors, globalStyles } = useTheme();

  const contentStyle = {
    width: '100%',
    maxWidth: layout.maxContentWidth,
    alignSelf: 'center',
    paddingVertical: layout.screenPaddingY,
    paddingHorizontal: margins ? layout.screenMargin : 0,
  } as const;

  return (
    <SafeAreaView
      edges={['top', 'right', 'bottom', 'left']}
      style={[globalStyles.flex1, { backgroundColor: colors.background }]}
    >
      <KeyboardAvoidingView
        behavior={Platform.OS === 'ios' ? 'padding' : undefined}
        style={globalStyles.flex1}
      >
        {scroll ? (
          <ScrollView
            keyboardShouldPersistTaps="handled"
            contentContainerStyle={[contentStyle, { flexGrow: 1 }]}
          >
            {children}
          </ScrollView>
        ) : (
          <View style={[globalStyles.flex1, contentStyle]}>{children}</View>
        )}
      </KeyboardAvoidingView>
    </SafeAreaView>
  );
}
