import {Text } from "react-native";
import { useTheme } from "../../context/ThemeContext";
import ModulView  from "../../components/atoms/ModulView";
import ModulLayout from "../../components/atoms/ModulLayout";
import ModulHeader from "../../components/organisms/ModulHeader";
import ModulText from "../../components/atoms/ModulText";

export default function Home() {
  const { globalTextStyles } = useTheme();
  return (
    <ModulLayout>
        <ModulHeader
          title="Inicio"
          leftAction="menu"
          showNotifications
          onNotificationsPress={() => {
          }}
        />
      <ModulView p16>
        <ModulText variant={"sectionTitle"}>
          Bienvenido
        </ModulText>
      </ModulView>
    </ModulLayout>
  );
}
