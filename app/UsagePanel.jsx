import React, { useEffect, useState } from 'react';

function windowName(minutes) {
  if (minutes === 10080) return 'Semanal';
  if (minutes === 43200) return '30 dias';
  return minutes >= 60 ? `${Math.round(minutes / 60)}h` : `${minutes} min`;
}

function resetText(seconds, now) {
  if (!seconds) return 'Renovação não informada';
  const minutes = Math.max(0, Math.ceil((seconds * 1000 - now) / 60_000));
  if (!minutes) return 'Aguardando renovação';
  if (minutes >= 1440) return `Renova em ${Math.floor(minutes / 1440)}d ${Math.floor(minutes % 1440 / 60)}h`;
  return `Renova em ${Math.floor(minutes / 60)}h ${minutes % 60}m`;
}

export default function UsagePanel({ now }) {
  const [usage, setUsage] = useState(null);
  const [refresh, setRefresh] = useState(0);
  const [loading, setLoading] = useState(true);
  useEffect(() => {
    const controller = new AbortController();
    let busy = false;
    let active = true;
    async function read() {
      if (busy || document.hidden) return;
      busy = true; setLoading(true);
      try {
        const response = await fetch('/api/usage', { signal: controller.signal, cache: 'no-store' });
        if (!response.ok) throw new Error('unavailable');
        const value = await response.json();
        if (active) setUsage(value.codex || null);
      } catch { if (active) setUsage(null); }
      finally { busy = false; if (active) setLoading(false); }
    }
    read();
    const timer = setInterval(read, 60_000);
    const visible = () => { if (!document.hidden) read(); };
    document.addEventListener('visibilitychange', visible);
    return () => { active = false; controller.abort(); clearInterval(timer); document.removeEventListener('visibilitychange', visible); };
  }, [refresh]);

  const windows = Array.isArray(usage?.windows) ? usage.windows : [];
  return <section className="ai-usage" aria-label="Consumo das inteligências artificiais">
    <div className="usage-heading"><span>CONSUMO DE IA</span><button onClick={() => setRefresh(value => value + 1)} disabled={loading} aria-label="Atualizar consumo das IAs">{loading ? 'Consultando…' : 'Atualizar ↻'}</button></div>
    <div className="provider-usage"><div className="provider-title"><strong>Codex</strong><span>{usage?.status === 'connected' ? 'Conta conectada' : loading ? 'Consultando conta' : 'Sem leitura'}</span></div>
      {usage?.status === 'connected' && windows.length ? windows.map(window => <div className="quota-row" key={window.id}>
        <div className="quota-label"><span>{windowName(window.minutes)}</span><strong>{Math.round(window.usedPercent)}% <small>usado</small></strong></div>
        <div className="quota-track" role="progressbar" aria-label={`Codex ${windowName(window.minutes)}`} aria-valuemin="0" aria-valuemax="100" aria-valuenow={window.usedPercent}><span style={{ width: `${window.usedPercent}%` }} /></div>
        <span className="quota-reset">{resetText(window.resetsAt, now.getTime())}</span>
      </div>) : <p className="provider-message">{loading ? 'Lendo os limites da conta…' : usage?.message || 'Conexão de consumo indisponível nesta prévia.'}</p>}
    </div>
    <div className="provider-usage claude"><div className="provider-title"><strong>Claude</strong><span>Não conectado</span></div><p className="provider-message">Integração pendente · consumo não disponível</p></div>
    {usage?.updatedAt && <span className="usage-updated">Consultado às {new Date(usage.updatedAt).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })} · atualização a cada minuto</span>}
  </section>;
}
