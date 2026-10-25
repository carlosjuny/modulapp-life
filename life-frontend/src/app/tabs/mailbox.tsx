import { useState } from 'react';
import { useLocalSearchParams, useRouter } from 'expo-router';

import ModulButton from '../../components/atoms/ModulButton';
import ModulInput from '../../components/atoms/ModulInput';
import ModulLayout from '../../components/atoms/ModulLayout';
import ModulView from '../../components/atoms/ModulView';
import ModulText from '../../components/atoms/ModulText';
import ModulListItem from '../../components/organisms/ModulListItem';
import ModulModal from '../../components/organisms/ModulModal';
import ModulSidebar from '../../components/organisms/ModulSidebar';

export default function Home() {
  const router = useRouter();
  const [menuOpen, setMenuOpen] = useState(false);
  const [quickOpen, setQuickOpen] = useState(false);
  const [lastQuick, setLastQuick] = useState<string | undefined>();
  const [title, setTitle] = useState('');

  const { quick } = useLocalSearchParams<{ quick?: string }>();

  if (quick && quick !== lastQuick) {
    setLastQuick(quick);
    setQuickOpen(true);
  }

  const closeQuick = () => {
    setTitle('');
    setQuickOpen(false);
  };

  return (
    <ModulLayout
      header={{
        title: 'Buzon',
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
      <ModulListItem
        title="Tareas"
        onPress={() => router.push('/tabs/mailbox/tasks' as never)}
      />

      <ModulModal
        visible={quickOpen}
        onClose={closeQuick}
        title="Nueva tarea"
        mode="bottom"
      >
        <ModulInput
          autoFocus
          placeholder="¿Qué hay que hacer?"
          value={title}
          onChangeText={setTitle}
          returnKeyType="done"
        />
        <ModulButton
          title="Guardar"
          fullWidth
          disabled={!title.trim()}
          onPress={closeQuick}
        />
      </ModulModal>
    </ModulLayout>
  );
}
