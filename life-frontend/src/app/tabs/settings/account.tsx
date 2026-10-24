import { router } from 'expo-router';

import ModulLayout from '../../../components/atoms/ModulLayout';
import { strings } from '../../../constants/strings';

export default function Account() {
  return (
    <ModulLayout
      header={{
        title: strings.screens.account,
        leftAction: 'back',
        showNotifications: false,
        onBackPress: () => router.back(),
      }}
    >
      {null}
    </ModulLayout>
  );
}
