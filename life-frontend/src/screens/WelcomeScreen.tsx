import { View } from "react-native";
import Text from "../components/atoms/ModulText";
import AppIcon from "../components/atoms/AppIcon";
import { HomeIcon } from "@solar-icons/react-native/linear";

export default function WelcomeScreen() {
  return (
    <View>
      <Text variant="screenTitle">
        <AppIcon icon={HomeIcon} />
        Bienvenido
      </Text>
    </View>
  );
}
