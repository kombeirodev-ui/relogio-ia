import { defineConfig } from 'vite';
import { readUsage } from './server/usage.mjs';

function connect(server) {
  server.middlewares.use(async (request, response, next) => {
    if (request.url?.split('?')[0] !== '/api/usage') return next();
    const host = request.headers.host || '';
    if (!/^(127\.0\.0\.1|localhost):\d+$/.test(host) || (request.headers.origin && request.headers.origin !== `http://${host}`)) {
      response.writeHead(403); response.end(); return;
    }
    if (request.method !== 'GET') { response.writeHead(405); response.end(); return; }
    response.setHeader('Content-Type', 'application/json; charset=utf-8');
    response.setHeader('Cache-Control', 'no-store');
    try { response.end(JSON.stringify(await readUsage())); }
    catch { response.statusCode = 503; response.end(JSON.stringify({ error: 'Consulta indisponível.' })); }
  });
}

export default defineConfig({ plugins: [{ name: 'local-usage', configureServer: connect, configurePreviewServer: connect }] });
