import { spawn } from 'node:child_process';

export function normalizeLimits(result) {
  const buckets = result?.rateLimitsByLimitId ? Object.entries(result.rateLimitsByLimitId) : [['codex', result?.rateLimits]];
  const windows = [];
  for (const [id, bucket] of buckets) {
    for (const key of ['primary', 'secondary']) {
      const window = bucket?.[key];
      if (typeof window?.usedPercent !== 'number' || !Number.isFinite(window.usedPercent) || window.usedPercent < 0 || window.usedPercent > 100 || !(window.windowDurationMins > 0)) continue;
      windows.push({ id: `${id}-${key}`, usedPercent: window.usedPercent, minutes: window.windowDurationMins, resetsAt: Number.isFinite(window.resetsAt) ? window.resetsAt : null });
    }
  }
  return windows;
}

export function readCodexLimits() {
  return new Promise((resolve, reject) => {
    const child = spawn('codex', ['app-server', '--listen', 'stdio://'], { windowsHide: true, stdio: ['pipe', 'pipe', 'ignore'] });
    let finished = false;
    let buffer = '';
    const timer = setTimeout(() => finish(new Error('timeout')), 12_000);
    function finish(error, value) {
      if (finished) return;
      finished = true;
      clearTimeout(timer);
      child.stdin.destroy();
      child.kill();
      error ? reject(error) : resolve(value);
    }
    const send = message => child.stdin.write(JSON.stringify(message) + '\n');
    child.on('error', () => finish(new Error('unavailable')));
    child.stdin.on('error', () => finish(new Error('unavailable')));
    child.on('exit', () => { if (!finished) finish(new Error('closed')); });
    child.stdout.on('data', chunk => {
      buffer += chunk.toString();
      if (buffer.length > 500_000) { finish(new Error('oversize')); return; }
      let end;
      while ((end = buffer.indexOf('\n')) >= 0) {
        const line = buffer.slice(0, end); buffer = buffer.slice(end + 1);
        let message;
        try { message = JSON.parse(line); } catch { continue; }
        if (message.id === 1) {
          if (message.error) { finish(new Error('initialize')); return; }
          send({ method: 'initialized' });
          send({ id: 2, method: 'account/rateLimits/read' });
        }
        if (message.id === 2) {
          if (message.error) finish(new Error('account'));
          else finish(null, normalizeLimits(message.result));
          return;
        }
      }
    });
    child.on('spawn', () => send({ id: 1, method: 'initialize', params: { clientInfo: { name: 'relogio_ia_usage', title: 'Relógio IA', version: '0.1.0' } } }));
  });
}

let cached;
let lastAttempt = 0;
let pending;
export async function readUsage() {
  if (cached && Date.now() - lastAttempt < 60_000) return cached;
  if (pending) return pending;
  lastAttempt = Date.now();
  pending = (async () => {
    try {
      const windows = await readCodexLimits();
      cached = { codex: { status: windows.length ? 'connected' : 'unavailable', windows, updatedAt: Date.now(), message: windows.length ? '' : 'A conta não informou limites nesta consulta.' } };
    } catch {
      cached = { codex: { status: 'unavailable', windows: [], updatedAt: null, message: 'Não foi possível consultar o Codex. Confira se o aplicativo está instalado e conectado à sua conta.' } };
    } finally { pending = null; }
    return cached;
  })();
  return pending;
}
