import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'
import federation from "@originjs/vite-plugin-federation";

// Permissive CORS in dev: avoids 401/CORS edge cases when host is localhost, 127.0.0.1, or ::1.
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  const apiGatewayTarget = (env.VITE_API_BASE_URL || 'http://localhost:5000').replace(/\/$/, '')

  return {
    plugins: [
        react(),
        federation({
            name: 'promotions_app',
            filename: 'remoteEntry.js',
            exposes: {
                './Promotions': './src/components/Promotions.jsx',
                './CustomerRelation': './src/components/CustomerRelation.jsx',
                './Participant': './src/components/Participant.jsx',
                './DeliveryPoint': './src/components/DeliveryPoint.jsx',
                './Products': './src/components/Products.jsx',
            },
            shared: {
                react: { singleton: true },
                'react-dom': { singleton: true }
            }
        })
    ],
    server: {
        host: true,
        port: 5002,
        strictPort: true,
        cors: true,
        // Use '/api/' not '/api': Vite matches url.startsWith(context). Chunk files like /api-BY_gXMS7.js
        // would be proxied to the gateway and return 404. Preview inherits this proxy (see Vite resolvePreviewOptions).
        proxy: {
            '/api/': {
                target: apiGatewayTarget,
                changeOrigin: true,
                secure: false,
            }
        }
    },
    preview: {
        host: true,
        port: 5002,
        strictPort: true,
        cors: true,
    },
    build: {
        modulePreload: false,
        target: 'esnext',
        minify: false,
        cssCodeSplit: false,
        assetsDir: ''
    }
  }
})
