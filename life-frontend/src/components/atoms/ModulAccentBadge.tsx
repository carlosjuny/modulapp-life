import React from 'react';
import Svg, { Defs, LinearGradient, Path, Stop } from 'react-native-svg';
import { lightColors } from '../../theme/colors';

type Props = { from: string; to: string; id: string; size?: number };

const SHAPE =
  'M97.98 35.9 A50 50 0 0 1 32.9 97.98 A24 24 0 0 0 2.02 64.1 A50 50 0 0 1 64.1 2.02 A24 24 0 0 0 97.98 32.9 Z';

export default function ModulAccentBadge({ from, to, id, size = 48 }: Props) {
  return (
    // El viewBox deja margen abajo y a los lados para que la sombra no se corte
    <Svg width={size} height={size} viewBox="-10 -6 120 124">
      <Defs>
        <LinearGradient id={id} x1="0" y1="0" x2="1" y2="0">
          <Stop offset="0" stopColor={from} />
          <Stop offset="1" stopColor={to} />
        </LinearGradient>
      </Defs>

      {/* Sombra: tres capas con bordes cada vez más suaves */}
      <Path
        d={SHAPE}
        fill={lightColors.black}
        fillOpacity={0.06}
        stroke={lightColors.black}
        strokeOpacity={0.04}
        strokeWidth={14}
        strokeLinejoin="round"
        transform="translate(0 7)"
      />
      <Path
        d={SHAPE}
        fill={lightColors.black}
        fillOpacity={0.08}
        stroke={lightColors.black}
        strokeOpacity={0.06}
        strokeWidth={8}
        strokeLinejoin="round"
        transform="translate(0 6)"
      />
      <Path
        d={SHAPE}
        fill={lightColors.black}
        fillOpacity={0.14}
        transform="translate(0 4)"
      />

      <Path d={SHAPE} fill={`url(#${id})`} />
    </Svg>
  );
}
