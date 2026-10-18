import {Text } from "react-native";
import { useTheme } from "../../context/ThemeContext";
import ModulView  from "../../components/atoms/ModulView";
import AppIcon from "../../components/atoms/ModulIcon";
import ModulLayout from "../../components/atoms/ModulLayout";
import { HomeIcon } from "@solar-icons/react-native/linear";

export default function Moduls() {
  const { globalTextStyles } = useTheme();
  return (
    <ModulLayout>
      <ModulView>
        <Text style={[globalTextStyles.textPrimary]}>
          <AppIcon icon={HomeIcon} />
          Bienvenido
        </Text>
      </ModulView>
    </ModulLayout>
  );
}
