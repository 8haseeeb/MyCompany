import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'
import federation from "@originjs/vite-plugin-federation";

// https://vite.dev/config/
// Promotions remote must be served by `vite build && vite preview` on port 5002 (see apps/promotions).
//
// IMPORTANT: Do not point VITE_PROMOTIONS_PREVIEW_TARGET at the API gateway (:5000 / :5089) — you get 401 on chunks.
// Default dev remote is same-origin /promotions/* (proxied to preview). If you set VITE_PROMOTIONS_MFE_ORIGIN,
// use the same host you open in the browser (localhost vs 127.0.0.1) or dynamic import() may fail cross-origin.
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')

  const isGatewayUrl = (url) => /:(5089|5000)(\/|$)/.test(url || '')

  // Proxy /promotions → this origin ONLY (static MFE preview). Never use VITE_API_BASE_URL or gateway URL here.
  let promoPreviewTarget = (env.VITE_PROMOTIONS_PREVIEW_TARGET || 'http://127.0.0.1:5002').replace(/\/$/, '')
  if (isGatewayUrl(promoPreviewTarget)) {
    console.warn(
      '[host vite] VITE_PROMOTIONS_PREVIEW_TARGET must be the promotions Vite preview (5002), not the API gateway (:5000 / :5089). Using http://127.0.0.1:5002.'
    )
    promoPreviewTarget = 'http://127.0.0.1:5002'
  }

  const mfeOriginOverride = env.VITE_PROMOTIONS_MFE_ORIGIN?.replace(/\/$/, '')
  let promotionsRemote
  if (mode === 'development') {
    if (mfeOriginOverride) {
      if (isGatewayUrl(mfeOriginOverride)) {
        console.warn(
          '[host vite] VITE_PROMOTIONS_MFE_ORIGIN must not be the API gateway (:5000 / :5089). Using same-origin /promotions/remoteEntry.js'
        )
        promotionsRemote = '/promotions/remoteEntry.js'
      } else {
        promotionsRemote = `${mfeOriginOverride}/remoteEntry.js`
      }
    } else {
      // Same-origin /promotions → Vite proxy → preview :5002. Avoids cross-origin dynamic import()
      // (e.g. page at http://localhost:5001 but chunks at http://127.0.0.1:5002 → browser treats as different origins → "Failed to fetch").
      promotionsRemote = '/promotions/remoteEntry.js'
    }
  } else {
    promotionsRemote = '/promotions/remoteEntry.js'
  }

  return {
  plugins: [
    react(),
    federation({
      name: 'host_app',
      remotes: {
        promotions_app: promotionsRemote,
      },

      shared: {
        react: { singleton: true },
        'react-dom': { singleton: true }
      }
    })
  ],
  server: {
    port: 5001,
    proxy: {
      // Before /api/: federation chunks rewritten to preview can be named api-*.js (must not match /api/ proxy).
      '/promotions': {
        target: promoPreviewTarget,
        changeOrigin: true,
        secure: false,
        rewrite: (p) => p.replace(/^\/promotions/, ''),
        configure: (proxy) => {
          proxy.on('error', (err) => {
            console.error('[host vite] /promotions proxy →', promoPreviewTarget, err.message)
          })
        },
      },
      '/api/': {
        // docker-compose publishes api-gateway as localhost:5089 (see repo docker-compose). Override via VITE_API_BASE_URL in .env.local if needed.
        target: (env.VITE_API_BASE_URL || 'http://localhost:5089').replace(/\/$/, ''),
        changeOrigin: true,
        secure: false,
        configure: (proxy) => {
          proxy.on('error', (err, _req, res) => {
            console.error('[host vite] /api/ proxy error → check gateway is running and VITE_API_BASE_URL matches its host port (Docker: :5089).', err.message)
            if (res && !res.headersSent) {
              res.writeHead(502, { 'Content-Type': 'application/json' })
              res.end(JSON.stringify({
                message: 'API proxy could not reach the gateway. Set VITE_API_BASE_URL in apps/host/.env.local (e.g. http://localhost:5089 for Docker).',
                detail: err.message
              }))
            }
          })
        },
      },
    }
  },
  build: {
    target: 'esnext'
  }
}
})
