import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

// SSL-сертифікат потрібен лише для локального dev-сервера (npm run dev).
// Під час production-збірки (npm run build, наприклад на Render) dotnet недоступний,
// тому ця функція викликається тільки в режимі 'serve'.
function getDevHttpsConfig() {
    // Знаходимо системну папку з SSL-сертифікатами .NET
    const baseFolder =
        env.APPDATA !== undefined && env.APPDATA !== ''
            ? `${env.APPDATA}/ASP.NET/https`
            : `${env.HOME}/.aspnet/https`;

    const certificateName = "film.client";
    const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
    const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

    if (!fs.existsSync(baseFolder)) {
        fs.mkdirSync(baseFolder, { recursive: true });
    }

    // Якщо сертифікат відсутній - експортуємо його через dotnet dev-certs
    if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
        if (0 !== child_process.spawnSync('dotnet', [
            'dev-certs',
            'https',
            '--export-path',
            certFilePath,
            '--format',
            'Pem',
            '--no-password',
        ], { stdio: 'inherit' }).status) {
            throw new Error("Не вдалося створити SSL сертифікат.");
        }
    }

    return {
        key: fs.readFileSync(keyFilePath),
        cert: fs.readFileSync(certFilePath),
    };
}

export default defineConfig(({ command }) => ({
    plugins: [react()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        port: 3001,
        strictPort: true,
        proxy: {
            '/api': {
                target: 'https://localhost:5050',
                secure: false,
                changeOrigin: true
            }
        },
        https: command === 'serve' ? getDevHttpsConfig() : undefined
    }
}));
