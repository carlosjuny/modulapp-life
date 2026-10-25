import { useLocalSearchParams, useRouter } from 'expo-router';

import ModulButton from '../../../../components/atoms/ModulButton';
import ModulLayout from '../../../../components/atoms/ModulLayout';
import ModulText from '../../../../components/atoms/ModulText';
import ModulView from '../../../../components/atoms/ModulView';

export default function TaskDetail() {
  const router = useRouter();
  const { title, assignee, date } = useLocalSearchParams<{
    title: string;
    assignee: string;
    date: string;
  }>();

  return (
    <ModulLayout
      scroll
      margins
      header={{
        title: title ?? 'Tarea',
        subtitle: `${assignee ?? ''} · ${date ?? ''}`,
        leftAction: 'back',
        showNotifications: false,
        onBackPress: () => router.back(),
      }}
    >
      <ModulView gap16>
        <ModulView gap4>
          <ModulText variant="sectionTitle">Descripción</ModulText>
          <ModulText>Detalle de ejemplo de la tarea.</ModulText>
        </ModulView>

        <ModulView gap4>
          <ModulText variant="sectionTitle">Responsable</ModulText>
          <ModulText>{assignee}</ModulText>
        </ModulView>
      </ModulView>

      <ModulView row wrap gap12 mt24>
        <ModulButton title="Editar" variant="secondary" />
        <ModulButton title="Completar" />
      </ModulView>
    </ModulLayout>
  );
}
