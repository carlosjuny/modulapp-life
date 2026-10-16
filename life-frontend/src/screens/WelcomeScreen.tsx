import {Text, View} from "react-native";
import { useTheme } from  '../context/ThemeContext';

export default function WelcomeScreen() {
  const { globalStyles, globalTextStyles } = useTheme();
  return (
    <View>
      <Text style={[globalTextStyles.textXl, globalTextStyles.textPrimary]}>
        Bienvenido
      </Text>
    </View>
  );
}
