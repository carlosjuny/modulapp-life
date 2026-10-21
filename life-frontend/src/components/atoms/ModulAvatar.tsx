import React from 'react';
import { Image } from 'react-native';

import { useTheme } from '../../context/ThemeContext';
import ModulText from './ModulText';
import ModulView from './ModulView';

const AVATAR_SIZES = { sm: 32, md: 40, lg: 56 };

type AvatarProps = {
  name: string;
  uri?: string;
  size?: keyof typeof AVATAR_SIZES;
};

const getInitials = (name: string) =>
  name
    .trim()
    .split(/\s+/)
    .slice(0, 2)
    .map((part) => part.charAt(0).toUpperCase())
    .join('');

export default function ModulAvatar({ name, uri, size = 'md' }: AvatarProps) {
  const { globalTextStyles } = useTheme();
  const dimension = AVATAR_SIZES[size];

  return (
    <ModulView
      bgPrimary
      alignCenter
      justifyCenter
      accessibilityRole="image"
      accessibilityLabel={name}
      style={{
        width: dimension,
        height: dimension,
        borderRadius: dimension / 2,
        overflow: 'hidden',
      }}
    >
      {uri ? (
        <Image source={{ uri }} style={{ width: dimension, height: dimension }} />
      ) : (
        <ModulText variant="label" style={globalTextStyles.textOnPrimary}>
          {getInitials(name)}
        </ModulText>
      )}
    </ModulView>
  );
}
