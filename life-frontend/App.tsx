import { ThemeProvider } from './src/context/ThemeContext';
import AppLayout from "./src/components/atoms/AppLayout";
import WelcomeScreen from "./src/screens/WelcomeScreen";

export default function App() {
  return (
    <ThemeProvider>
      <AppLayout>
        <WelcomeScreen/>
      </AppLayout>
    </ThemeProvider>
  );
}

