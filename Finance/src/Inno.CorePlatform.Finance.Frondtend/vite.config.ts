import _ from 'lodash';
import { defineConfig, loadEnv } from 'vite';
import type { ConfigEnv, UserConfig } from 'vite';

export default defineConfig(async (config: ConfigEnv): UserConfig => {
  const viteConfig = await import('@inno/inno-mc-vue3/template/vite.config.js');
  const { mode } = config;
  const env = loadEnv(mode, process.cwd());
  const mergeConfig = _.merge(viteConfig.default(config), {
    base: env.VITE_APP_BASE_PATH, // 子应用前缀，不要与现有子应用有冲突
    server: {
      host: '0.0.0.0',
      port: env.VITE_APP_PORT // 运行端口，不要与现有子应用有冲突
    }
  });

  return mergeConfig;
});
