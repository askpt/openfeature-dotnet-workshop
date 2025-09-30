import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), "");

  const goffServiceUrl = SanitizeUrl(
    process.env.ConnectionStrings__goff || "none"
  );

  // Only define GOFF service URL if it's not "none"
  const defineConfig: any = {};
  if (goffServiceUrl !== "none") {
    defineConfig["import.meta.env.VITE_GOFF_SERVICE_URL"] =
      JSON.stringify(goffServiceUrl);
  }

  return {
    plugins: [react()],
    server: {
      port: parseInt(env.VITE_PORT),
      proxy: {
        "/api": {
          target:
            process.env.services__apiservice__https__0 ||
            process.env.services__apiservice__http__0,
          changeOrigin: true,
          rewrite: (path) => path.replace(/^\/api/, ""),
          secure: false,
        },
      },
    },
    build: {
      outDir: "dist",
      rollupOptions: {
        input: "./index.html",
      },
    },
    define: {
      // Expose the GOFF service URL to the client
      ...defineConfig,
    },
  };
});

function SanitizeUrl(goffServiceUrl: string) {
  // Implement your URL sanitization logic here (Endpoint=https://localhost:8080)
  // Remove Endpoint= from the URL
  return goffServiceUrl.replace("Endpoint=", "");
}
