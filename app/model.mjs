export const STORAGE_KEY = 'relogio-ia:skin:v1';
export const METRICS = { cpu: 'CPU', gpu: 'GPU', ram: 'RAM', disk: 'Disco' };
export const DEFAULTS = { metrics: ['cpu', 'gpu', 'ram'], accent: '#e9bd69', font: 'sans', expanded: false, background: '', fade: 0, textColor: '#ffffff', clockStyle: 'segments', atlas: '' };

export function readSettings(raw) {
  let value;
  try { value = JSON.parse(raw); } catch { value = null; }
  if (!value || typeof value !== 'object' || Array.isArray(value)) value = {};
  const background = typeof value.background === 'string' && value.background.length <= 2_000_000 && /^data:image\/jpeg;base64,[A-Za-z0-9+/=]+$/.test(value.background) ? value.background : '';
  return {
    metrics: Array.isArray(value.metrics) ? [...new Set(value.metrics.filter(key => typeof key === 'string' && Object.hasOwn(METRICS, key)))] : [...DEFAULTS.metrics],
    accent: typeof value.accent === 'string' && /^#[a-fA-F0-9]{6}$/.test(value.accent) ? value.accent : DEFAULTS.accent,
    font: ['sans', 'mono', 'serif', 'condensed', 'rounded'].includes(value.font) ? value.font : DEFAULTS.font,
    expanded: value.expanded === true,
    background,
    fade: typeof value.fade === 'number' && Number.isFinite(value.fade) ? Math.min(1, Math.max(0, value.fade)) : 0,
    textColor: typeof value.textColor === 'string' && /^#[a-fA-F0-9]{6}$/.test(value.textColor) ? value.textColor : DEFAULTS.textColor,
    clockStyle: ['segments', 'text', 'outline', 'atlas'].includes(value.clockStyle) ? value.clockStyle : DEFAULTS.clockStyle,
    atlas: typeof value.atlas === 'string' && value.atlas.length <= 1_000_000 && /^data:image\/png;base64,[A-Za-z0-9+/=]+$/.test(value.atlas) ? value.atlas : '',
  };
}

export function atlasError(width, height) {
  if (!Number.isInteger(width) || width % 11 !== 0 || width < 110) return 'A largura deve conter 11 células iguais, na ordem 0123456789: (exemplo: 1100 × 150).';
  if (width > 4096 || height < 16 || height > 512) return 'Use uma faixa de até 4096 pixels de largura e entre 16 e 512 de altura.';
  return '';
}

export function cropRect(width, height, zoom, horizontal, vertical) {
  const ratio = 2;
  const safeZoom = Math.min(3, Math.max(1, Number(zoom) || 1));
  const cropWidth = Math.min(width, height * ratio) / safeZoom;
  const cropHeight = cropWidth / ratio;
  const position = number => Math.min(1, Math.max(0, Number(number) || 0));
  return { x: (width - cropWidth) * position(horizontal), y: (height - cropHeight) * position(vertical), width: cropWidth, height: cropHeight };
}

export function imageError(file) {
  if (!file || !['image/jpeg', 'image/png', 'image/webp'].includes(file.type)) return 'Escolha uma imagem PNG, JPG ou WebP. Animações ficam para a próxima etapa.';
  if (file.size > 5 * 1024 * 1024) return 'A imagem precisa ter até 5 MB.';
  return '';
}
