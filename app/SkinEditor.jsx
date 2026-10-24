import React, { useEffect, useRef, useState } from 'react';
import { cropRect, imageError } from './model.mjs';

export default function SkinEditor({ onApply }) {
  const [bitmap, setBitmap] = useState(null);
  const [position, setPosition] = useState({ zoom: 1, x: 0.5, y: 0.5 });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const canvas = useRef(null);
  const generation = useRef(0);

  useEffect(() => () => { generation.current++; }, []);
  useEffect(() => () => bitmap?.close(), [bitmap]);
  useEffect(() => {
    if (!bitmap || !canvas.current) return;
    const crop = cropRect(bitmap.width, bitmap.height, position.zoom, position.x, position.y);
    const context = canvas.current.getContext('2d');
    context.fillStyle = '#222b2b';
    context.fillRect(0, 0, 1200, 600);
    context.drawImage(bitmap, crop.x, crop.y, crop.width, crop.height, 0, 0, 1200, 600);
  }, [bitmap, position]);

  async function choose(event) {
    const file = event.target.files?.[0];
    event.target.value = '';
    if (!file) return;
    const current = ++generation.current;
    const problem = imageError(file);
    if (problem) { setLoading(false); setError(problem); return; }
    setError(''); setLoading(true);
    try {
      const image = await createImageBitmap(file);
      if (current !== generation.current) { image.close(); return; }
      if (image.width * image.height > 25_000_000) { image.close(); throw new Error('Escolha uma imagem de até 25 megapixels para manter o editor leve.'); }
      setBitmap(image);
      setPosition({ zoom: 1, x: 0.5, y: 0.5 });
    } catch (reason) {
      if (current === generation.current) setError(reason.message.startsWith('Escolha') ? reason.message : 'Não foi possível abrir essa imagem. Tente outro arquivo PNG, JPG ou WebP.');
    } finally { if (current === generation.current) setLoading(false); }
  }

  function apply() {
    try {
      const output = canvas.current.toDataURL('image/jpeg', 0.82);
      if (output.length > 2_000_000) { setError('O recorte ficou grande demais. Tente uma imagem mais simples.'); return; }
      onApply(output);
    } catch { setError('Não foi possível salvar o recorte. Tente outra imagem.'); }
  }

  return <div className="skin-editor">
    <label className="file-picker">Selecionar arquivo<input type="file" accept="image/png,image/jpeg,image/webp" onChange={choose} aria-label="Selecionar foto de fundo" /></label>
    <p className="file-hint">PNG, JPG ou WebP · até 5 MB · recorte 2:1<br />Animações não são preservadas nesta etapa.</p>
    {loading && <p role="status">Preparando imagem…</p>}
    {error && <p className="error-message" role="alert">{error}</p>}
    {bitmap && <><canvas ref={canvas} width="1200" height="600" role="img" aria-label="Prévia do recorte da imagem" />{[['zoom', 'Zoom', 1, 3], ['x', 'Posição horizontal', 0, 1], ['y', 'Posição vertical', 0, 1]].map(([key, label, min, max]) => <label className="range-field" key={key}><span>{label}</span><input type="range" min={min} max={max} step="0.01" value={position[key]} onChange={event => setPosition({ ...position, [key]: Number(event.target.value) })} /></label>)}<button className="primary-button apply-button" disabled={loading} onClick={apply}>Aplicar recorte ✓</button></>}
  </div>;
}
