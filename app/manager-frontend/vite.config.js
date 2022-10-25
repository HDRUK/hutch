import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import { config as loadEnv } from "dotenv";
import { env } from "process";
import tsconfigPaths from "vite-tsconfig-paths";
import pluginRaw from "vite-plugin-raw";
import svgr from "vite-plugin-svgr";

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  const config = {
    plugins: [react(), svgr(), tsconfigPaths(), pluginRaw({ match: /\.md$/ })],
  };

  switch (mode) {
    case "development": {
      // `.aspnet/aspnetcore-react` writes dev-cert details to this file
      loadEnv({ path: ".env.development.local" });

      const proxyTarget = env.ASPNETCORE_HTTPS_PORT
        ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
        : env.ASPNETCORE_URLS
        ? env.ASPNETCORE_URLS.split(";")[0]
        : "https://localhost:7745";

      return {
        ...config,
        server: {
          port: 45588,
          https: {
            key: env.SSL_KEY_FILE,
            cert: env.SSL_CRT_FILE,
          },
          proxy: {
            "/api": {
              target: proxyTarget,
              secure: false,
            },
          },
        },
      };
    }
    default:
      return config;
  }
});
