import test from 'node:test';
import assert from 'node:assert/strict';
const module = await import('./usage.mjs').catch(() => ({}));

test('limites ausentes não viram 0% nem um provedor conectado', () => {
  assert.equal(typeof module.normalizeLimits, 'function');
  assert.deepEqual(module.normalizeLimits({}), []);
  assert.deepEqual(module.normalizeLimits({ rateLimits: { primary: null } }), []);
});

test('normalização expõe apenas dados de consumo e respeita limites múltiplos', () => {
  assert.equal(typeof module.normalizeLimits, 'function');
  const result = module.normalizeLimits({ secret: 'not for UI', rateLimitsByLimitId: { codex: { primary: { usedPercent: 24, windowDurationMins: 300, resetsAt: 123 }, secondary: { usedPercent: 39, windowDurationMins: 10080, resetsAt: 456 } } } });
  assert.equal(result.length, 2);
  assert.equal(result[0].usedPercent, 24);
  assert.equal(result[1].minutes, 10080);
  assert.equal(JSON.stringify(result).includes('secret'), false);
});

test('valor ausente ou fora da faixa não é apresentado como leitura válida', () => {
  assert.equal(typeof module.normalizeLimits, 'function');
  assert.deepEqual(module.normalizeLimits({ rateLimits: { primary: { usedPercent: null, windowDurationMins: 300 }, secondary: { usedPercent: 150, windowDurationMins: 300 } } }), []);
});
