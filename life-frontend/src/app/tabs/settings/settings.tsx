import { router } from 'expo-router';
import { UserCircleIcon } from '@solar-icons/react-native/linear';

import ModulIcon from '../../../components/atoms/ModulIcon';
import ModulLayout from '../../../components/atoms/ModulLayout';
import ModulListItem from '../../../components/organisms/ModulListItem';
import { useTheme } from '../../../context/ThemeContext';
import { strings } from '../../../constants/strings';

export default function Settings() {
  const { colors } = useTheme();

  return (
    <ModulLayout
      header={{
        title: strings.screens.settings,
        leftAction: 'back',
        showNotifications: false,
        onBackPress: () => router.back(),
      }}
    >
      <ModulListItem
        title={strings.screens.account}
        left={<ModulIcon icon={UserCircleIcon} color={colors.textSecondary} />}
        onPress={() => router.push('/tabs/settings/account' as never)}
      />
    </ModulLayout>
  );
}
