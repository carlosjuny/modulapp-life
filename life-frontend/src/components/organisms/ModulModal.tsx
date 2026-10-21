import React from 'react';
import { Modal, Pressable } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';

import { useTheme } from '../../context/ThemeContext';
import ModulText from '../atoms/ModulText';
import ModulView from '../atoms/ModulView';

type ModalProps = {
  visible: boolean;
  onClose: () => void;
  title?: string;
  mode?: 'center' | 'bottom';
  children: React.ReactNode;
};

export default function ModulModal({
                                     visible,
                                     onClose,
                                     title,
                                     mode = 'center',
                                     children,
                                   }: ModalProps) {
  const { globalStyles, globalTextStyles } = useTheme();
  const insets = useSafeAreaInsets();
  const isBottom = mode === 'bottom';

  return (
    <Modal
      visible={visible}
      transparent
      animationType={isBottom ? 'slide' : 'fade'}
      onRequestClose={onClose}
    >
      <ModulView flex1 justifyCenter={!isBottom} justifyEnd={isBottom} p16={!isBottom}>
        <Pressable
          onPress={onClose}
          accessibilityRole="button"
          accessibilityLabel="Cerrar"
          style={[globalStyles.absoluteFill, globalStyles.backdrop]}
        />

        <ModulView
          bgTertiary
          p16
          gap12
          roundedLg={!isBottom}
          roundedTopLg={isBottom}
          style={isBottom && { paddingBottom: 16 + insets.bottom }}
        >
          <ModulView row alignCenter justifyBetween>
            <ModulText variant="sectionTitle" style={globalStyles.flex1}>
              {title}
            </ModulText>
            <Pressable
              onPress={onClose}
              accessibilityRole="button"
              accessibilityLabel="Cerrar"
              style={[
                globalStyles.minH44,
                globalStyles.minW44,
                globalStyles.alignCenter,
                globalStyles.justifyCenter,
              ]}
            >
              <ModulText variant="button" style={globalTextStyles.textBrand}>
                Cerrar
              </ModulText>
            </Pressable>
          </ModulView>

          {children}
        </ModulView>
      </ModulView>
    </Modal>
  );
}
