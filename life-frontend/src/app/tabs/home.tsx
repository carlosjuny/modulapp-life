import { useState } from "react";
import { useTheme } from "../../context/ThemeContext";
import ModulView  from "../../components/atoms/ModulView";
import ModulLayout from "../../components/atoms/ModulLayout";
import ModulHeader from "../../components/organisms/ModulHeader";
import ModulText from "../../components/atoms/ModulText";
import ModulSidebar from "../../components/organisms/ModulSidebar";

export default function Home() {
  const { globalTextStyles } = useTheme();
  const [menuOpen, setMenuOpen] = useState(false);
  return (
    <ModulLayout>
        <ModulHeader
          title="Inicio"
          leftAction="menu"
          showNotifications
          onMenuPress={() => setMenuOpen(true)}
          onNotificationsPress={() => {
          }}
        />
      <ModulSidebar visible={menuOpen} onClose={() => setMenuOpen(false)} />
      <ModulView p16>
        <ModulText variant={"sectionTitle"}>
          Bienvenido
        </ModulText>
      </ModulView>
    </ModulLayout>
  );
}
