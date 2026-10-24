import test from 'node:test';
import assert from 'node:assert/strict';

const model = await import('./model.mjs').catch(() => ({}));

test('configuração corrompida restaura uma seleção utilizável', () => {
  assert.equal(typeof model.readSettings, 'function', 'leitor de preferências ainda não implementado');
  const result = model.readSettings('{broken');
  assert.deepEqual(result.metrics, ['cpu', 'gpu', 'ram']);
  assert.equal(result.expanded, false);
});

test('dados persistidos não inserem métricas, cores ou fontes arbitrárias', () => {
  assert.equal(typeof model.readSettings, 'function');
  const result = model.readSettings(JSON.stringify({ metrics: ['cpu', 'cpu', 'secret', 'disk'], accent: 'url(bad)', font: 'unknown', expanded: 'false' }));
  assert.deepEqual(result.metrics, ['cpu', 'disk']);
  assert.equal(result.accent, '#e9bd69');
  assert.equal(result.font, 'sans');
  assert.equal(result.expanded, false);
});

test('usuário pode ocultar todos os indicadores e manter a escolha', () => {
  assert.equal(typeof model.readSettings, 'function');
  const result = model.readSettings(JSON.stringify({ metrics: [], accent: '#11aa88', font: 'mono', expanded: true }));
  assert.deepEqual(result.metrics, []);
  assert.equal(result.font, 'mono');
  assert.equal(result.expanded, true);
});

test('objetos inválidos dentro da seleção não impedem restaurar preferências', () => {
  assert.doesNotThrow(() => model.readSettings(JSON.stringify({ metrics: [{ toString: null }, 'gpu'] })));
  assert.deepEqual(model.readSettings(JSON.stringify({ metrics: [{ toString: null }, 'gpu'] })).metrics, ['gpu']);
});

test('recorte não ultrapassa uma imagem vertical mesmo com zoom e posição extremos', () => {
  assert.equal(typeof model.cropRect, 'function');
  const crop = model.cropRect(600, 1200, 2, 1, 1);
  assert.equal(crop.width / crop.height, 2);
  assert.ok(crop.x >= 0 && crop.y >= 0);
  assert.ok(crop.x + crop.width <= 600);
  assert.ok(crop.y + crop.height <= 1200);
  assert.equal(crop.x + crop.width, 600);
  assert.equal(crop.y + crop.height, 1200);
});

test('recorte central sem zoom aproveita toda largura disponível da imagem vertical', () => {
  assert.equal(typeof model.cropRect, 'function');
  assert.deepEqual(model.cropRect(600, 1200, 1, 0.5, 0.5), { x: 0, y: 450, width: 600, height: 300 });
});

test('foto aceita somente formatos estáticos e rejeita arquivos acima de 5 MB', () => {
  assert.equal(typeof model.imageError, 'function');
  assert.equal(model.imageError({ type: 'image/jpeg', size: 1024 }), '');
  assert.match(model.imageError({ type: 'image/gif', size: 1024 }), /PNG, JPG ou WebP/);
  assert.match(model.imageError({ type: 'image/png', size: 6 * 1024 * 1024 }), /5 MB/);
});

test('foto não tem escurecimento obrigatório e o usuário pode escolher zero', () => {
  assert.equal(model.readSettings(null).fade, 0);
  assert.equal(model.readSettings(JSON.stringify({ fade: 0 })).fade, 0);
  assert.equal(model.readSettings(JSON.stringify({ fade: 2 })).fade, 1);
  assert.equal(model.readSettings(JSON.stringify({ fade: -1 })).fade, 0);
});

test('padrões de números são persistidos sem descartar preferências antigas', () => {
  const value = model.readSettings(JSON.stringify({ clockStyle: 'segments', accent: '#11aa88', fade: 0.35, textColor: '#ffffff' }));
  assert.equal(value.clockStyle, 'segments');
  assert.equal(value.fade, 0.35);
  assert.equal(value.textColor, '#ffffff');
  assert.equal(value.accent, '#11aa88');
});

test('faixa personalizada exige onze células completas e tamanho controlado', () => {
  assert.equal(typeof model.atlasError, 'function');
  assert.equal(model.atlasError(1100, 150), '');
  assert.match(model.atlasError(1099, 150), /11/);
  assert.match(model.atlasError(1100, 1000), /512/);
});
