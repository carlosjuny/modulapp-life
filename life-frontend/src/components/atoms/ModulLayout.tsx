import React from 'react';
import { View } from 'react-native';
import {
  SafeAreaProvider,
  SafeAreaView,
} from 'react-native-safe-area-context';

import { useTheme } from '../../context/ThemeContext';

type AppLayoutProps = {
  children: React.ReactNode;
};

export default function AppLayout({ children }: AppLayoutProps) {
  const { colors, globalStyles } = useTheme();

  return (
    <SafeAreaProvider
      style={{ flex: 1, backgroundColor: colors.background }}
    >
      <SafeAreaView
        edges={['top', 'right', 'bottom', 'left']}
        style={[
          globalStyles.flex1,
          { backgroundColor: colors.background },
        ]}
      >
        <View
          style={[
            globalStyles.flex1,
            globalStyles.py16,
            {
              width: '100%',
              maxWidth: 720,
              alignSelf: 'center',
            },
          ]}
        >
          {children}
        </View>
      </SafeAreaView>
    </SafeAreaProvider>
  );
}
