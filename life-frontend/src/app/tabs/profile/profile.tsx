import { router } from 'expo-router';

import ModulLayout from '../../../components/atoms/ModulLayout';
import { strings } from '../../../constants/strings';

export default function Profile() {
  return (
    <ModulLayout
      header={{
        title: strings.screens.profile,
        leftAction: 'back',
        showNotifications: false,
        onBackPress: () => router.back(),
      }}
    >
      {null}
    </ModulLayout>
  );
}
