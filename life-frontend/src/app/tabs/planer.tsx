import { useState } from 'react';

import ModulLayout from '../../components/atoms/ModulLayout';
import ModulView from '../../components/atoms/ModulView';
import ModulText from '../../components/atoms/ModulText';
import ModulSidebar from '../../components/organisms/ModulSidebar';

export default function Home() {
  const [menuOpen, setMenuOpen] = useState(false);

  return (
    <ModulLayout
      header={{
        title: 'Agenda',
        leftAction: 'menu',
        showNotifications: true,
        onMenuPress: () => setMenuOpen(true),
        onNotificationsPress: () => {},
      }}
    >
      <ModulSidebar
        visible={menuOpen}
        onClose={() => setMenuOpen(false)}
      />

      <ModulView px12>
        <ModulText variant="sectionTitle">
          Bienvenido
        </ModulText>
      </ModulView>
    </ModulLayout>
  );
}
