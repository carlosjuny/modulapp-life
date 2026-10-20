const API_URL = process.env.EXPO_PUBLIC_API_URL;
const TIMEOUT_MS = 8000;

export class ApiError extends Error {
  constructor(message: string) {
    super(message);
    this.name = 'ApiError';
  }
}

export async function apiGet(path: string): Promise<string> {
  if (!API_URL) {
    throw new ApiError('La aplicación no tiene configurada la dirección del servidor.');
  }

  const controller = new AbortController();
  const timer = setTimeout(() => controller.abort(), TIMEOUT_MS);

  try {
    const response = await fetch(`${API_URL}${path}`, { signal: controller.signal });
    const body = await response.text();

    // /health responde 503 con el texto "Unhealthy"; se devuelve el cuerpo sin tratarlo como error de red.
    if (!response.ok && body.trim() !== 'Unhealthy') {
      throw new ApiError('El servidor respondió con un problema. Intentá de nuevo más tarde.');
    }
    return body.trim();
  } catch (error) {
    if (error instanceof ApiError) throw error;
    throw new ApiError('No se pudo conectar con el servidor. Revisá tu conexión.');
  } finally {
    clearTimeout(timer);
  }
}

export const getHealth = () => apiGet('/health');
