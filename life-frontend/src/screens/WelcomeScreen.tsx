import {Text, View} from "react-native";
import { useTheme } from  '../context/ThemeContext';
import AppIcon from "../components/atoms/AppIcon";
import { HomeIcon } from "@solar-icons/react-native/linear";

export default function WelcomeScreen() {
  const { globalStyles, globalTextStyles } = useTheme();
  return (
    <View>
      <Text style={[globalTextStyles.textXl, globalTextStyles.textPrimary]}>
        <AppIcon icon={HomeIcon} />
        Bienvenido
      </Text>
    </View>
  );
}
