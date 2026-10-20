import { Tabs } from 'expo-router';

import ModulMenu from '../../components/organisms/ModulMenu';
import { useTheme } from '../../context/ThemeContext';

export default function TabsLayout() {
  const { colors } = useTheme();

  return (
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
      <Tabs.Screen name="diagnostico" options={{ title: 'Diagnóstico', href: null }} />
    </Tabs>
  );
}
