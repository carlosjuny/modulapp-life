import { StyleSheet } from 'react-native';

export const appearanceStyles = StyleSheet.create({
  // Colores de tema
  accents: { rowGap: 16 },
  ring: { width: 60, height: 60, borderWidth: 2 },
  check: { right: -2, bottom: -2, width: 20, height: 20 },
  checkMark: { fontSize: 12, lineHeight: 14, fontWeight: '700' },

  // Vista previa del celular
  phone: { maxWidth: 88, aspectRatio: 0.7, borderRadius: 16 },
  screen: { padding: 5 },
  speaker: { alignSelf: 'center', width: 14, height: 3, borderRadius: 2, opacity: 0.6 },
  header: { height: 6, borderRadius: 3, width: '60%', opacity: 0.8 },
  card: { height: 14, borderRadius: 5 },
  nav: { marginTop: 'auto', justifyContent: 'space-around' },
  tab: { width: 8, height: 3, borderRadius: 2, opacity: 0.6 },
  fab: { width: 12, height: 12, borderRadius: 6 },
});
