import React, { useEffect, useState } from 'react';
import type { ComponentProps } from 'react';
import { Animated, Easing } from 'react-native';

import { HomeIcon } from '@solar-icons/react-native/linear';

import ModulIcon from './ModulIcon';

type IconComponent = React.ComponentType<ComponentProps<typeof HomeIcon>>;

type IconCycleProps = {
  icons: IconComponent[];
  color?: string;
  active?: boolean;
  interval?: number;
};

// Muestra un icono a la vez: el actual sale girando y entra el siguiente.
export default function ModulIconCycle({
                                         icons,
                                         color,
                                         active = true,
                                         interval = 2600,
                                       }: IconCycleProps) {
  const [swap] = useState(() => new Animated.Value(1));
  const [index, setIndex] = useState(0);

  useEffect(() => {
    if (!active || icons.length < 2) return;

    let running = true;
    const id = setInterval(() => {
      Animated.timing(swap, {
        toValue: 0,
        duration: 180,
        useNativeDriver: true,
      }).start(({ finished }) => {
        if (!running || !finished) return;
        setIndex((i) => (i + 1) % icons.length);
        Animated.timing(swap, {
          toValue: 1,
          duration: 280,
          easing: Easing.out(Easing.back(1.6)),
          useNativeDriver: true,
        }).start();
      });
    }, interval);

    return () => {
      running = false;
      clearInterval(id);
      swap.setValue(1);
      setIndex(0);
    };
  }, [active, icons.length, interval, swap]);

  const Icon = icons[index] ?? icons[0];

  return (
    <Animated.View
      style={{
        opacity: swap,
        transform: [
          { scale: swap.interpolate({ inputRange: [0, 1], outputRange: [0.5, 1] }) },
          {
            rotate: swap.interpolate({ inputRange: [0, 1], outputRange: ['-90deg', '0deg'] }),
          },
        ],
      }}
    >
      <ModulIcon icon={Icon} color={color} />
    </Animated.View>
  );
}
