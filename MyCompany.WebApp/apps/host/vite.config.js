import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import federation from "@originjs/vite-plugin-federation";

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    federation({
      name: 'host_app',
      remotes: {
        'promotions_app': 'http://127.0.0.1:5003/remoteEntry.js',
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
      '/api': {
        target: 'http://localhost:5089',
        changeOrigin: true,
        secure: false,
      }
    }
  },
  build: {
    target: 'esnext'
  }
})
