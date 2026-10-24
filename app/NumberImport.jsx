import React, { useState } from 'react';
import { atlasError } from './model.mjs';

export default function NumberImport({ onApply, hasAtlas, onRemove }) {
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);
  async function choose(event) {
    const file = event.target.files?.[0]; event.target.value = '';
    if (!file) return;
    setError('');
    if (file.type !== 'image/png' || file.size > 1024 * 1024) { setError('Use PNG de até 1 MB, de preferência com fundo transparente.'); return; }
    setBusy(true);
    let bitmap;
    try {
      bitmap = await createImageBitmap(file);
      const problem = atlasError(bitmap.width, bitmap.height);
      if (problem) throw new Error(problem);
      const canvas = document.createElement('canvas');
      canvas.width = 704; canvas.height = 96;
      const context = canvas.getContext('2d');
      const cellWidth = bitmap.width / 11;
      const scale = Math.min(64 / cellWidth, 96 / bitmap.height);
      const width = cellWidth * scale;
      const height = bitmap.height * scale;
      for (let index = 0; index < 11; index++) context.drawImage(bitmap, index * cellWidth, 0, cellWidth, bitmap.height, index * 64 + (64 - width) / 2, (96 - height) / 2, width, height);
      const output = canvas.toDataURL('image/png');
      if (output.length > 1_000_000) throw new Error('A faixa ficou grande demais para salvar. Reduza a resolução do PNG.');
      onApply(output);
    } catch (reason) { setError(reason.message?.includes('11') || reason.message?.includes('faixa') || reason.message?.includes('512') ? reason.message : 'Não foi possível ler o PNG. Confira o formato da faixa.'); }
    finally { bitmap?.close(); setBusy(false); }
  }
  return <div className="number-import"><p className="field-hint">Seus próprios números: envie uma faixa PNG com <strong>11 células iguais</strong>, na ordem:</p><code className="digit-guide">0 1 2 3 4 5 6 7 8 9 :</code><p className="file-hint">Exemplo: 1100 × 150 px (cada célula 100 × 150). Sem IA para interpretar: cada célula é um símbolo. O desenho original mantém suas cores.</p><label className="file-picker">{busy ? 'Lendo faixa…' : 'Importar números desenhados'}<input aria-label="Importar faixa de números" type="file" accept="image/png" disabled={busy} onChange={choose} /></label>{hasAtlas && <button className="text-button" onClick={onRemove}>Remover números importados</button>}{error && <p role="alert" className="error-message">{error}</p>}</div>;
}
