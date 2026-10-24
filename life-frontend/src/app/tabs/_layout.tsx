import { View } from 'react-native';
import { Tabs } from 'expo-router';

import ModulFab from '../../components/atoms/ModulFab';
import ModulMenu from '../../components/organisms/ModulMenu';
import { useTheme } from '../../context/ThemeContext';

export default function TabsLayout() {
  const { colors } = useTheme();

  return (
    <View style={{ flex: 1 }}>
      <Tabs
        tabBar={(props) => <ModulMenu {...props} />}
        screenOptions={{
          headerShown: false,
          sceneStyle: {
            backgroundColor: colors.background,
          },
        }}
      >
        <Tabs.Screen name="home" options={{ title: 'Inicio' }} />
        <Tabs.Screen name="mailbox" options={{ title: 'Buzon' }} />
        <Tabs.Screen name="planer" options={{ title: 'Agenda' }} />
        <Tabs.Screen name="moduls" options={{ title: 'Módulos' }} />
        <Tabs.Screen name="profile/profile" options={{ title: 'Perfil' }} />
        <Tabs.Screen name="settings/account" options={{ title: 'Ajustes de la cuenta' }} />
        <Tabs.Screen name="settings/settings" options={{ title: 'Configuración' }} />
        <Tabs.Screen name="diagnostico" options={{ title: 'Diagnóstico', href: null }} />
      </Tabs>
      <ModulFab onPress={() => {}} />
    </View>
  );
}
