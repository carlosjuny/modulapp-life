import React from 'react';
import { Pressable } from 'react-native';

import { useTheme } from '../../context/ThemeContext';
import { strings } from '../../constants/strings';
import { appearanceStyles } from '../../theme/appearanceStyles';
import {
  ACCENT_COLORS,
  THEME_ACCENTS,
  darkColors,
  lightColors,
  type ThemeColors,
  type ThemePreference,
} from '../../theme/colors';
import ModulAccentBadge from '../atoms/ModulAccentBadge';
import ModulSwitch from '../atoms/ModulSwitch';
import ModulText from '../atoms/ModulText';
import ModulView from '../atoms/ModulView';

const MODES: { value: ThemePreference; label: string }[] = [
  { value: 'light', label: strings.appearance.light },
  { value: 'dark', label: strings.appearance.dark },
  { value: 'system', label: strings.appearance.auto },
];

function PhoneScreen({ c }: { c: ThemeColors }) {
  return (
    <ModulView
      flex1
      gap4
      style={[appearanceStyles.screen, { backgroundColor: c.background }]}
    >
      <ModulView style={[appearanceStyles.speaker, { backgroundColor: c.textSecondary }]} />
      <ModulView style={[appearanceStyles.header, { backgroundColor: c.text }]} />
      <ModulView style={[appearanceStyles.card, { backgroundColor: c.surface }]} />
      <ModulView style={[appearanceStyles.card, { backgroundColor: c.surface }]} />
      <ModulView row alignCenter style={appearanceStyles.nav}>
        <ModulView style={[appearanceStyles.tab, { backgroundColor: c.textSecondary }]} />
        <ModulView style={[appearanceStyles.fab, { backgroundColor: c.primary }]} />
        <ModulView style={[appearanceStyles.tab, { backgroundColor: c.textSecondary }]} />
      </ModulView>
    </ModulView>
  );
}

function PhonePreview({ mode, selected }: { mode: ThemePreference; selected: boolean }) {
  const { colors } = useTheme();

  return (
    <ModulView
      w100
      overflowHidden
      style={[
        appearanceStyles.phone,
        {
          borderColor: selected ? colors.primary : colors.borderStrong,
          borderWidth: selected ? 4 : 3,
        },
      ]}
    >
      {mode === 'light' && <PhoneScreen c={lightColors} />}
      {mode === 'dark' && <PhoneScreen c={darkColors} />}
      {mode === 'system' && (
        <ModulView flex1 row>
          <PhoneScreen c={lightColors} />
          <PhoneScreen c={darkColors} />
        </ModulView>
      )}
    </ModulView>
  );
}

export function AccentPicker() {
  const { colors, globalStyles, accent, setAccent } = useTheme();

  return (
    <ModulView gap8>
      <ModulText variant="sectionTitle">{strings.appearance.accentTitle}</ModulText>
      <ModulText variant="bodySecondary">{strings.appearance.accentHelp}</ModulText>
      <ModulView
        accessibilityRole="radiogroup"
        row
        wrap
        mt8
        style={appearanceStyles.accents}
      >
        {THEME_ACCENTS.map((item) => {
          const selected = item === accent;
          const name = strings.appearance.accentNames[item];
          const [from, to] = ACCENT_COLORS[item].gradient;

          return (
            <Pressable
              key={item}
              onPress={() => setAccent(item)}
              accessibilityRole="radio"
              accessibilityLabel={strings.appearance.accentLabel(name)}
              accessibilityState={{ selected }}
              style={[globalStyles.w25, globalStyles.minH44, globalStyles.alignCenter]}
            >
              <ModulView
                alignCenter
                justifyCenter
                roundedFull
                style={[
                  appearanceStyles.ring,
                  { borderColor: selected ? colors.text : colors.transparent },
                ]}
              >
                <ModulAccentBadge from={from} to={to} id={`accent-${item}`} />
                {selected && (
                  <ModulView
                    absolute
                    alignCenter
                    justifyCenter
                    roundedFull
                    style={[appearanceStyles.check, { backgroundColor: colors.text }]}
                  >
                    <ModulText
                      accessible={false}
                      style={[appearanceStyles.checkMark, { color: colors.background }]}
                    >
                      ✓
                    </ModulText>
                  </ModulView>
                )}
              </ModulView>
            </Pressable>
          );
        })}
      </ModulView>
    </ModulView>
  );
}

export function ModePicker() {
  const { globalStyles, preference, setPreference } = useTheme();

  return (
    <ModulView gap8>
      <ModulText variant="sectionTitle">{strings.appearance.modeTitle}</ModulText>
      <ModulText variant="bodySecondary">{strings.appearance.modeHelp}</ModulText>
      <ModulView accessibilityRole="radiogroup" row gap12 mt8>
        {MODES.map(({ value, label }) => {
          const selected = value === preference;
          return (
            <Pressable
              key={value}
              onPress={() => setPreference(value)}
              accessibilityRole="radio"
              accessibilityLabel={label}
              accessibilityState={{ selected }}
              style={[
                globalStyles.flex1,
                globalStyles.minH44,
                globalStyles.alignCenter,
                globalStyles.gap8,
              ]}
            >
              <PhonePreview mode={value} selected={selected} />
              <ModulText
                variant="label"
                style={{ fontWeight: selected ? '700' : '400', textAlign: 'center' }}
              >
                {label}
              </ModulText>
            </Pressable>
          );
        })}
      </ModulView>
    </ModulView>
  );
}

export function ContrastToggle() {
  const { highContrast, setHighContrast } = useTheme();

  return (
    <ModulView gap8>
      <ModulText variant="sectionTitle">{strings.appearance.contrastTitle}</ModulText>
      <ModulText variant="bodySecondary">{strings.appearance.contrastHelp}</ModulText>
      <ModulSwitch
        value={highContrast}
        onValueChange={setHighContrast}
        label={strings.appearance.contrastSwitch}
      />
    </ModulView>
  );
}

// Pantalla de Configuración: Color de tema, Apariencia y Contraste (orden del ticket)
export default function ModulAppearanceSettings() {
  return (
    <ModulView gap24 mt16>
      <AccentPicker />
      <ModePicker />
      <ContrastToggle />
    </ModulView>
  );
}
