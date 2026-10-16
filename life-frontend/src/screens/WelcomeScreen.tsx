import {Text, View} from "react-native";
import { useTheme } from  '../context/ThemeContext';

export default function WelcomeScreen() {
  const { globalStyles, globalTextStyles } = useTheme();
  return (
    <View style={[globalStyles.flex1, globalStyles.bg, globalStyles.p16]}>
      <Text style={[globalTextStyles.textXl, globalTextStyles.textPrimary]}>
        Bienvenido
      </Text>
    </View>
  );
}
