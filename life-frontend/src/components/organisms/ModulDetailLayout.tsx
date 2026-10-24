import React from 'react';

import ModulLayout from '../atoms/ModulLayout';
import ModulView from '../atoms/ModulView';

type ModulDetailLayoutProps = {
  title: string;
  /** Metadatos del elemento (por ejemplo, "Ana · Hoy"). */
  meta?: string;
  onBackPress?: () => void;
  /** Secciones de contenido. */
  children: React.ReactNode;
  /** Zona de acciones (ModulButton). */
  actions?: React.ReactNode;
};

export default function ModulDetailLayout({
                                            title,
                                            meta,
                                            onBackPress,
                                            children,
                                            actions,
                                          }: ModulDetailLayoutProps) {
  return (
    <ModulLayout
      scroll
      margins
      header={{
        title,
        subtitle: meta,
        leftAction: 'back',
        showNotifications: false,
        onBackPress,
      }}
    >
      <ModulView gap16>{children}</ModulView>

      {actions && (
        <ModulView row wrap gap12 mt24>
          {actions}
        </ModulView>
      )}
    </ModulLayout>
  );
}
