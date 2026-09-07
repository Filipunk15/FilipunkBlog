// Zkopíruje potřebné soubory z node_modules do wwwroot/lib/ (git-ignored).
// Spouští se přes `npm run assets:build` a z MSBuild targetu při buildu.
import { mkdirSync, copyFileSync, existsSync } from 'node:fs';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = resolve(dirname(fileURLToPath(import.meta.url)), '..');

const jobs = [
  {
    from: 'node_modules/@highlightjs/cdn-assets/highlight.min.js',
    to: 'wwwroot/lib/hljs/highlight.min.js',
  },
  {
    from: 'node_modules/@highlightjs/cdn-assets/styles/github-dark.min.css',
    to: 'wwwroot/lib/hljs/github-dark.min.css',
  },
];

for (const { from, to } of jobs) {
  const src = resolve(root, from);
  const dest = resolve(root, to);
  if (!existsSync(src)) {
    console.error(`copy-assets: chybí zdroj ${from} — spusť 'npm install'`);
    process.exit(1);
  }
  mkdirSync(dirname(dest), { recursive: true });
  copyFileSync(src, dest);
  console.log(`copy-assets: ${from} → ${to}`);
}
