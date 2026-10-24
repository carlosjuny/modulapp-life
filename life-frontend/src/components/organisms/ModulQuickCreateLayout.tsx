import React from 'react';

import ModulButton from '../atoms/ModulButton';
import ModulView from '../atoms/ModulView';

type ModulQuickCreateLayoutProps = {
  /** Campos del formulario; el primero recibe el foco. */
  children: React.ReactNode;
  onSave: () => void;
  saveTitle?: string;
  saveDisabled?: boolean;
};

export default function ModulQuickCreateLayout({
                                                 children,
                                                 onSave,
                                                 saveTitle = 'Guardar',
                                                 saveDisabled = false,
                                               }: ModulQuickCreateLayoutProps) {
  const fields = React.Children.toArray(children);
  const [first, ...rest] = fields;

  return (
    <ModulView gap12>
      <ModulView gap12>
        {React.isValidElement<{ autoFocus?: boolean }>(first)
          ? React.cloneElement(first, { autoFocus: true })
          : first}
        {rest}
      </ModulView>

      <ModulButton
        title={saveTitle}
        onPress={onSave}
        disabled={saveDisabled}
        fullWidth
      />
    </ModulView>
  );
}
