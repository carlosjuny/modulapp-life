import React from 'react';
import { ScrollView } from 'react-native';

import ModulLayout from '../atoms/ModulLayout';
import ModulView from '../atoms/ModulView';
import type { ModulHeaderProps } from './ModulHeader';

type ModulListLayoutProps = {
  header: ModulHeaderProps;
  /** Zona de filtros opcional (chips, buscador). */
  filters?: React.ReactNode;
  /** Acción principal (por ejemplo, un ModulButton "Nueva tarea"). */
  action?: React.ReactNode;
  /** Filas del listado (ModulListItem). */
  children: React.ReactNode;
};

export default function ModulListLayout({
                                          header,
                                          filters,
                                          action,
                                          children,
                                        }: ModulListLayoutProps) {
  return (
    <ModulLayout header={header}>
      {filters && (
        <ModulView px16 py8 row wrap gap8>
          {filters}
        </ModulView>
      )}

      <ModulView flex1>
        <ScrollView keyboardShouldPersistTaps="handled">
          {children}
        </ScrollView>
      </ModulView>

      {action && (
        <ModulView px16 py12 borderTop>
          {action}
        </ModulView>
      )}
    </ModulLayout>
  );
}
