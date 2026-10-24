export const strings = {
  sidebar: {
    title: 'Menú',
    close: 'Cerrar menú',
    profileLabel: (name: string) => `Perfil de ${name}`,
    profileLoading: 'Cargando perfil…',
    profileFallbackName: 'Tu perfil',
    profileFallbackHint: 'No se pudo cargar tu información',
    settings: 'Configuración de la app',
    theme: 'Tema',
    signOut: 'Cerrar sesión',
  },
  screens: {
    profile: 'Perfil',
    account: 'Ajustes de la cuenta',
    settings: 'Configuración de la app',
  },

  appearance: {
    accentTitle: 'Color de tema',
    accentHelp: 'Elige un tema preferido para la app.',
    accentLabel: (name: string) => `Color de tema ${name}`,
    accentNames: {
      technology: 'Tecnología', life: 'Life', business: 'Negocios',
      services: 'Servicios', auto: 'Auto', ai: 'IA', sport: 'Deporte',
    },
    modeTitle: 'Apariencia',
    modeHelp:
      'Elige el modo claro u oscuro, o cambia el modo automáticamente según la configuración del sistema.',
    light: 'Claro',
    dark: 'Oscuro',
    auto: 'Auto',
    contrastTitle: 'Contraste',
    contrastHelp: 'Activa y desactiva el texto y los bordes con alto contraste.',
    contrastSwitch: 'Alto contraste para mayor accesibilidad',
    modalTitle: 'Personalizar tema',
  },
};
