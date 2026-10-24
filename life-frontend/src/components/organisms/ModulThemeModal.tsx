import React from 'react';
import { ScrollView, useWindowDimensions } from 'react-native';

import { strings } from '../../constants/strings';
import ModulView from '../atoms/ModulView';
import ModulModal from './ModulModal';
import { AccentPicker, ContrastToggle, ModePicker } from './ModulAppearanceSettings';

type Props = { visible: boolean; onClose: () => void };

export default function ModulThemeModal({ visible, onClose }: Props) {
  const { height } = useWindowDimensions();

  return (
    <ModulModal visible={visible} onClose={onClose} title={strings.appearance.modalTitle} mode="bottom">
      <ScrollView style={{ maxHeight: height * 0.9 }}>
        <ModulView gap24>
          <ModePicker />
          <AccentPicker />
          <ContrastToggle />
        </ModulView>
      </ScrollView>
    </ModulModal>
  );
}
