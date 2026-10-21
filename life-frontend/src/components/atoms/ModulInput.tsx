import React, { useState } from 'react';
import { Pressable, TextInput as RNTextInput, type TextInputProps } from 'react-native';

import { useTheme } from '../../context/ThemeContext';
import { MAX_FONT_SIZE_MULTIPLIER, typography } from '../../theme/typography';
import ModulText from './ModulText';
import ModulView from './ModulView';

export type InputType = 'text' | 'number' | 'decimal' | 'email' | 'password' | 'phone';

type InputProps = Omit<TextInputProps, 'style'> & {
  type?: InputType;
  label?: string;
  helperText?: string;
  errorText?: string;
};

const INPUT_CONFIG: Record<InputType, Partial<TextInputProps>> = {
  text: {},
  number: { keyboardType: 'number-pad' },
  decimal: { keyboardType: 'decimal-pad' },
  email: {
    keyboardType: 'email-address',
    autoCapitalize: 'none',
    autoCorrect: false,
    autoComplete: 'email',
  },
  password: {
    autoCapitalize: 'none',
    autoCorrect: false,
    autoComplete: 'password',
  },
  phone: { keyboardType: 'phone-pad', autoComplete: 'tel' },
};

const sanitize = (type: InputType, text: string) => {
  if (type === 'number') return text.replace(/\D/g, '');
  if (type === 'decimal') return text.replace(/[^0-9.]/g, '').replace(/(\..*)\./g, '$1');
  return text;
};

export default function ModulInput({
                                     type = 'text',
                                     label,
                                     helperText,
                                     errorText,
                                     editable = true,
                                     onChangeText,
                                     onFocus,
                                     onBlur,
                                     ...rest
                                   }: InputProps) {
  const { colors, globalStyles, globalTextStyles } = useTheme();
  const [focused, setFocused] = useState(false);
  const [hidden, setHidden] = useState(true);

  const isPassword = type === 'password';

  const borderStyle = errorText
    ? globalStyles.borderError
    : focused
      ? globalStyles.borderFocus
      : globalStyles.border;

  return (
    <ModulView gap4 style={!editable && globalStyles.opacityDisabled}>
      {label && <ModulText variant="label">{label}</ModulText>}

      <ModulView row alignCenter px12 rounded bg style={[borderStyle]}>
        <RNTextInput
          {...INPUT_CONFIG[type]}
          {...rest}
          secureTextEntry={isPassword && hidden}
          editable={editable}
          accessibilityLabel={rest.accessibilityLabel ?? label}
          accessibilityState={{ disabled: !editable }}
          allowFontScaling
          maxFontSizeMultiplier={MAX_FONT_SIZE_MULTIPLIER}
          placeholderTextColor={colors.textSecondary}
          onChangeText={(text) => onChangeText?.(sanitize(type, text))}
          onFocus={(e) => {
            setFocused(true);
            onFocus?.(e);
          }}
          onBlur={(e) => {
            setFocused(false);
            onBlur?.(e);
          }}
          style={[
            typography.body,
            globalTextStyles.textPrimary,
            globalStyles.flex1,
            globalStyles.minH44,
            globalStyles.py8,
          ]}
        />

        {isPassword && (
          <Pressable
            onPress={() => setHidden((value) => !value)}
            accessibilityRole="button"
            accessibilityLabel={hidden ? 'Mostrar contraseña' : 'Ocultar contraseña'}
            style={[globalStyles.minH44, globalStyles.minW44, globalStyles.alignCenter, globalStyles.justifyCenter]}
          >
            <ModulText variant="label" style={globalTextStyles.textBrand}>
              {hidden ? 'Mostrar' : 'Ocultar'}
            </ModulText>
          </Pressable>
        )}
      </ModulView>

      {errorText ? (
        <ModulText variant="bodySecondary" style={globalTextStyles.textDanger}>
          {errorText}
        </ModulText>
      ) : (
        helperText && <ModulText variant="bodySecondary">{helperText}</ModulText>
      )}
    </ModulView>
  );
}
