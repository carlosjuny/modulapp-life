import { useState } from 'react';
import { ScrollView } from 'react-native';
import { useRouter } from 'expo-router';

import ModulChip from '../../../../components/atoms/ModulChip';
import ModulLayout from '../../../../components/atoms/ModulLayout';
import ModulView from '../../../../components/atoms/ModulView';
import ModulListItem from '../../../../components/organisms/ModulListItem';

const FILTERS = ['Todas', 'Hoy', 'Mañana'];

const TASKS = [
  { id: '1', title: 'Comprar víveres', assignee: 'Ana', date: 'Hoy' },
  { id: '2', title: 'Pagar servicios', assignee: 'Carlos', date: 'Mañana' },
  { id: '3', title: 'Llevar el carro al taller', assignee: 'Ana', date: 'Viernes' },
  { id: '4', title: 'Reunión del colegio', assignee: 'Carlos', date: 'Lunes' },
];

export default function Tasks() {
  const router = useRouter();
  const [filter, setFilter] = useState('Todas');

  return (
    <ModulLayout
      header={{
        title: 'Tareas',
        leftAction: 'back',
        showNotifications: false,
      }}
    >
      <ModulView row wrap gap8 px16 py8>
        {FILTERS.map((item) => (
          <ModulChip
            key={item}
            label={item}
            selected={filter === item}
            onPress={() => setFilter(item)}
          />
        ))}
      </ModulView>

      <ModulView flex1>
        <ScrollView keyboardShouldPersistTaps="handled">
          {TASKS.map((task) => (
            <ModulListItem
              key={task.id}
              title={task.title}
              subtitle={task.assignee}
              right={<ModulChip label={task.date} />}
              onPress={() =>
                router.push({
                  pathname: '/tabs/mailbox/tasks/detail',
                  params: {
                    title: task.title,
                    assignee: task.assignee,
                    date: task.date,
                  },
                } as never)
              }
            />
          ))}
        </ScrollView>
        <ModulView mb48 />
      </ModulView>
    </ModulLayout>
  );
}
