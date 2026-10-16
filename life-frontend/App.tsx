import { ThemeProvider } from './src/context/ThemeContext';
import WelcomeScreen from "./src/screens/WelcomeScreen";

export default function App() {
  return (
    <ThemeProvider>
      <WelcomeScreen/>
    </ThemeProvider>
  );
}

