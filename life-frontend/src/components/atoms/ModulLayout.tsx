import React from 'react';
import {
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  View,
} from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';

import { useTheme } from '../../context/ThemeContext';
import { createModulLayoutStyles } from '../../styles/global';
import ModulHeader, {
  type ModulHeaderProps,
} from '../organisms/ModulHeader';

type ModulLayoutProps = {
  children: React.ReactNode;
  scroll?: boolean;
  margins?: boolean;
  header?: ModulHeaderProps;
};

export default function ModulLayout({
                                      children,
                                      scroll = false,
                                      margins = false,
                                      header,
                                    }: ModulLayoutProps) {
  const { colors } = useTheme();
  const insets = useSafeAreaInsets();

  const styles = createModulLayoutStyles(
    colors,
    insets.top,
    margins,
  );

  return (
    <View style={styles.root}>
      <KeyboardAvoidingView
        behavior={Platform.OS === 'ios' ? 'padding' : undefined}
        style={styles.keyboardAvoiding}
      >
        {header && <ModulHeader {...header} />}

        {scroll ? (
          <ScrollView
            keyboardShouldPersistTaps="handled"
            contentContainerStyle={styles.scrollContent}
          >
            {children}
          </ScrollView>
        ) : (
          <View style={styles.screenContent}>
            {children}
          </View>
        )}
      </KeyboardAvoidingView>
    </View>
  );
}
