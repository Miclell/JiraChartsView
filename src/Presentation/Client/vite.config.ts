import {defineConfig, loadEnv} from 'vite';
import react from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig(({mode}) => {
  const env = loadEnv(mode, process.cwd(), '');
  const proxyTarget = env.VITE_DEV_PROXY_TARGET ?? 'http://localhost:5085';
  const shouldProxy = !env.VITE_API_BASE_URL;
  const devServerPort = Number(env.VITE_DEV_SERVER_PORT ?? 5173);

  return {
    plugins: [react()],
    server: {
      port: devServerPort,
      proxy: shouldProxy
        ? {
            '/api': {
              target: proxyTarget,
              changeOrigin: true,
            },
          }
        : undefined,
    },
  };
});
