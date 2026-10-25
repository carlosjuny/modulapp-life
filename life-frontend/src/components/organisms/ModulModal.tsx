import React from 'react';
import { Modal, Pressable } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { CloseIcon } from '@solar-icons/react-native/linear';

import { useTheme } from '../../context/ThemeContext';
import ModulIcon from '../atoms/ModulIcon';
import ModulText from '../atoms/ModulText';
import ModulView from '../atoms/ModulView';

type ModalProps = {
  visible: boolean;
  onClose: () => void;
  title?: string;
  mode?: 'center' | 'bottom' | 'top';
  children: React.ReactNode;
};

export default function ModulModal({
                                     visible,
                                     onClose,
                                     title,
                                     mode = 'center',
                                     children,
                                   }: ModalProps) {
  const { colors, globalStyles } = useTheme();
  const insets = useSafeAreaInsets();
  const isCenter = mode === 'center';
  const isBottom = mode === 'bottom';
  const isTop = mode === 'top';

  return (
    <Modal
      visible={visible}
      transparent
      animationType={isBottom ? 'slide' : 'fade'}
      onRequestClose={onClose}
    >
      <ModulView
        flex1
        justifyCenter={isCenter}
        justifyEnd={isBottom}
        p16={isCenter}
        px16={isTop}
      >
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
          roundedLg={isCenter}
          roundedTopLg={isBottom}
          roundedBottomLg={isTop}
          style={[
            isBottom && { paddingBottom: 16 + insets.bottom },
            isTop && { paddingTop: 16 + insets.top },
          ]}
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
              <ModulIcon icon={CloseIcon} color={colors.text} />
            </Pressable>
          </ModulView>

          {children}
        </ModulView>
      </ModulView>
    </Modal>
  );
}
