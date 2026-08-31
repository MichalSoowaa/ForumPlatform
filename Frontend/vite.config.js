import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'


// Dev server proxies /api/* to the ASP.NET Core backend, so the fronted can call relative paths (e.g. fetch('/api/auth/login'))
// without dealing with CORS during local development. In production, the frontend is served separately and calls the API's real URL directly (see src/api/client.js)
export default defineConfig({
  plugins: [react()],
  servers: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'https://localhost:7074',
        changeOrigin: true,
        secure: false, // Accept the .NET dev HTTPS certificate
        rewrite: (path) => path.replace(/^\/api/, ''),
      },
    },
  },
});
