import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import federation from "@originjs/vite-plugin-federation";

export default defineConfig({
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
            },
            shared: {
                react: { singleton: true },
                'react-dom': { singleton: true }
            }
        })
    ],
    server: {
        port: 5002,
        cors: true,
        proxy: {
            '/api': {
                target: 'http://localhost:5089',
                changeOrigin: true,
                secure: false,
            }
        }
    },
    preview: {
        port: 5002,
        strictPort: true,
        cors: true,
    },
    build: {
        target: 'esnext',
        cssCodeSplit: false
    }
})
