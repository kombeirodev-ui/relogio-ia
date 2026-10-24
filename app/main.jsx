import React, { lazy, Suspense, useEffect, useState } from 'react';
import { createRoot } from 'react-dom/client';
import { DEFAULTS, METRICS, STORAGE_KEY, readSettings } from './model.mjs';
import './styles.css';
import UsagePanel from './UsagePanel.jsx';
import ClockDigits from './ClockDigits.jsx';

const SkinEditor = lazy(() => import('./SkinEditor.jsx'));
const NumberImport = lazy(() => import('./NumberImport.jsx'));
const fonts = { sans: 'Trebuchet MS, sans-serif', mono: 'Consolas, monospace', serif: 'Georgia, serif', condensed: 'Impact, sans-serif', rounded: 'Segoe UI, sans-serif' };

function App() {
  const [settings, setSettings] = useState(() => {
    try { return readSettings(localStorage.getItem(STORAGE_KEY)); } catch { return readSettings(null); }
  });
  const [now, setNow] = useState(new Date());
  const [editing, setEditing] = useState(false);
  const [notice, setNotice] = useState('');

  useEffect(() => {
    let timeout;
    const tick = () => {
      setNow(new Date());
      timeout = window.setTimeout(tick, 60_000 - Date.now() % 60_000);
    };
    timeout = window.setTimeout(tick, 60_000 - Date.now() % 60_000);
    const visible = () => { if (!document.hidden) { clearTimeout(timeout); tick(); } };
    document.addEventListener('visibilitychange', visible);
    return () => { clearTimeout(timeout); document.removeEventListener('visibilitychange', visible); };
  }, []);

  function change(patch) {
    const next = readSettings(JSON.stringify({ ...settings, ...patch }));
    setSettings(next);
    try { localStorage.setItem(STORAGE_KEY, JSON.stringify(next)); setNotice('Preferências salvas neste navegador.'); }
    catch { setNotice('A mudança foi aplicada, mas este navegador não permitiu salvar. Ela pode se perder ao fechar.'); }
  }

  function toggleMetric(key) {
    change({ metrics: settings.metrics.includes(key) ? settings.metrics.filter(metric => metric !== key) : [...settings.metrics, key] });
  }

  return <div className="workbench">
    <header className="page-header">
      <a className="wordmark" href="#inicio" aria-label="Relógio IA — início"><span className="brand-symbol" aria-hidden="true">◷</span> RELÓGIO IA<span className="version">/ 0.1</span></a>
      <span className="stage-label"><i /> Primeira interface</span>
    </header>

    <main id="inicio" className="layout">
      <section className="preview-section" aria-labelledby="preview-heading">
        <div className="section-caption"><span>01 / SEU WIDGET</span><span>LOCAL · SEM CHAMADAS DE IA</span></div>
        <h1 id="preview-heading">Seu canto.<br /><em>Do seu jeito.</em></h1>
        <p className="intro">O consumo das suas IAs, sempre à vista.<br className="desktop-break" /> Personalize sem perder o que importa.</p>

        <div className="preview-mat">
          <div className="mat-label">PRÉVIA INTERATIVA <span>↙ experimente os controles</span></div>
          <article className={`widget ${settings.expanded ? 'expanded' : ''} ${settings.background ? 'has-photo' : ''} font-${settings.font}`} style={{ '--accent': settings.accent, '--widget-font': fonts[settings.font], '--fade': settings.fade, '--text-color': settings.textColor }} aria-label="Prévia do widget">
            {settings.background && <div className="widget-photo" style={{ backgroundImage: `url("${settings.background}")` }} />}
            <div className="widget-content">
              <div className="widget-topline"><span className="widget-name"><span aria-hidden="true">◷</span> RELÓGIO IA</span><span className="widget-state">{settings.expanded ? 'DETALHES' : 'COMPACTO'}</span></div>
              <div className="widget-main">
                <div className="time-block"><time dateTime={now.toISOString()}><ClockDigits time={now.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })} style={settings.clockStyle} atlas={settings.atlas} /></time><span>{now.toLocaleDateString('pt-BR', { weekday: 'short', day: '2-digit', month: 'short' }).replaceAll('.', '')}</span></div>
                <div className="metric-list" aria-label="Indicadores selecionados">
                  {settings.metrics.length ? settings.metrics.map(key => <div className="metric" key={key} aria-label={`${METRICS[key]}: aguardando conexão`}><span>{METRICS[key]}</span><strong>—<small>%</small></strong><div className="metric-track" /></div>) : <p className="no-metrics">Só o relógio.<br />Indicadores ocultos.</p>}
                </div>
              </div>
              <UsagePanel now={now} />
              {settings.expanded && <div className="widget-details" id="widget-details"><div><span>HARDWARE</span><p>CPU, GPU, RAM e disco ainda não foram conectados ao Windows.</p></div><div><span>DADOS REAIS</span><p>O Codex informa o uso da conta, incluindo outras sessões. O fade altera só o fundo; as barras continuam visíveis.</p></div></div>}
              <div className="widget-footer"><span className="offline"><i /> Hardware ainda não conectado</span><button aria-expanded={settings.expanded} onClick={() => change({ expanded: !settings.expanded })}>{settings.expanded ? 'Recolher −' : 'Expandir +'}</button></div>
            </div>
          </article>
          <p className="mat-footnote">Prévia ao vivo · consumo de IA também no modo compacto.</p>
        </div>

        <div className="build-note"><span className="note-number">i</span><p><strong>O relógio continua à vista enquanto você edita.</strong><br />Codex com leitura de limites. Claude, sensores, música e janela de desktop ainda estão pendentes.</p></div>
      </section>

      <aside className="customizer" aria-labelledby="customizer-heading">
        <div className="panel-title"><div><span className="eyebrow">02 / PERSONALIZAR</span><h2 id="customizer-heading">Sua composição</h2></div><span aria-hidden="true">✳</span></div>
        <fieldset><legend><span className="step">01</span> Hardware opcional</legend><p className="field-hint">O consumo das IAs fica sempre visível. Escolha os indicadores extras.</p><div className="metric-options">{Object.entries(METRICS).map(([key, label]) => <label key={key} className="metric-option"><input type="checkbox" checked={settings.metrics.includes(key)} onChange={() => toggleMetric(key)} /><span>{label}</span><span aria-hidden="true" className="checkmark">{settings.metrics.includes(key) ? '✓' : '+'}</span></label>)}</div></fieldset>

        <fieldset><legend><span className="step">02</span> Cor de destaque</legend><div className="color-row">{['#e9bd69', '#93c5b4', '#97b8dd', '#e6a392'].map(color => <button key={color} className="swatch" style={{ background: color }} aria-label={`Usar cor ${color}`} aria-pressed={settings.accent === color} onClick={() => change({ accent: color })}>{settings.accent === color ? '✓' : ''}</button>)}<label className="custom-color">Outra cor<input type="color" aria-label="Cor personalizada" value={settings.accent} onChange={event => change({ accent: event.target.value })} /></label></div></fieldset>

        <fieldset><legend><span className="step">03</span> Números do relógio</legend><div className="clock-options">{[['segments', 'Rádio-relógio', '12:34'], ['text', 'Tipográfico', '12:34'], ['outline', 'Contorno', '12:34'], ...(settings.atlas ? [['atlas', 'Meus desenhos', '12:34']] : [])].map(([key, label, sample]) => <button key={key} aria-pressed={settings.clockStyle === key} onClick={() => change({ clockStyle: key })}><ClockDigits time={sample} style={key} atlas={settings.atlas} /><span>{label}</span></button>)}</div><details className="import-details"><summary>Importar meus próprios números</summary><Suspense fallback={<p>Carregando importação…</p>}><NumberImport hasAtlas={!!settings.atlas} onApply={atlas => change({ atlas, clockStyle: 'atlas' })} onRemove={() => change({ atlas: '', clockStyle: 'segments' })} /></Suspense></details></fieldset>

        <fieldset><legend><span className="step">04</span> Letras da interface</legend><div className="font-options">{[['sans', 'Clássica'], ['mono', 'Terminal'], ['serif', 'Editorial'], ['condensed', 'Condensada'], ['rounded', 'Suave']].map(([key, label]) => <button key={key} aria-pressed={settings.font === key} onClick={() => change({ font: key })}><b style={{ fontFamily: fonts[key] }}>Aa</b><span>{label}</span></button>)}</div></fieldset>

        <fieldset><legend><span className="step">05</span> Sua foto, sem filtro obrigatório</legend><p className="field-hint">A foto aparece com suas cores originais. Você escolhe quanto escurecer.</p><div className="background-actions"><button className="primary-button" onClick={() => setEditing(!editing)} aria-expanded={editing}>{editing ? 'Fechar editor' : settings.background ? 'Trocar imagem ↗' : 'Escolher imagem ↗'}</button>{settings.background && <button className="text-button" onClick={() => change({ background: '' })}>Remover fundo</button>}</div><label className="range-field fade-control"><span>Fade / escurecimento <output>{Math.round(settings.fade * 100)}%</output></span><input aria-label="Escurecimento da foto" type="range" min="0" max="100" step="1" value={Math.round(settings.fade * 100)} onChange={event => change({ fade: Number(event.target.value) / 100 })} /></label><p className="file-hint">0% = foto sem escurecimento · 100% = fundo preto</p><label className="text-color-control">Cor dos textos<input type="color" aria-label="Cor dos textos sobre a foto" value={settings.textColor} onChange={event => change({ textColor: event.target.value })} /></label>{editing && <Suspense fallback={<p className="field-hint">Abrindo editor…</p>}><SkinEditor onApply={background => { change({ background }); setEditing(false); }} /></Suspense>}</fieldset>

        <div className="saved-note"><span aria-hidden="true">↳</span><span role="status">{notice || 'Suas escolhas ficam salvas neste navegador.'}</span></div>
        <button className="reset-button" onClick={() => { if (window.confirm('Voltar às preferências iniciais e remover a imagem de fundo?')) { change(DEFAULTS); setEditing(false); } }}>Restaurar preferências iniciais</button>
      </aside>
    </main>
    <footer className="page-footer"><span>PEQUENO POR FORA. SEU POR DENTRO.</span><span>Fotos locais · Sem fontes externas · Sem IA para personalizar</span></footer>
  </div>;
}

createRoot(document.getElementById('root')).render(<App />);
