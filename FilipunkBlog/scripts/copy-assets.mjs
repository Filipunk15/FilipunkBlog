// Zkopíruje potřebné soubory z node_modules do wwwroot/lib/ (git-ignored).
// Spouští se přes `npm run assets:build` a z MSBuild targetu při buildu.
// Vše se servíruje z vlastní domény – žádné CDN / Google Fonts / FontAwesome kit.
import { mkdirSync, copyFileSync, existsSync, readdirSync } from 'node:fs';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = resolve(dirname(fileURLToPath(import.meta.url)), '..');

const jobs = [
  // highlight.js (načítá se líně jen na stránkách s bloky kódu, viz wwwroot/app.js)
  { from: 'node_modules/@highlightjs/cdn-assets/highlight.min.js', to: 'wwwroot/lib/hljs/highlight.min.js' },

  // Písma (variabilní Inter + Space Mono; latin + latin-ext kvůli české diakritice)
  { from: 'node_modules/@fontsource-variable/inter/files/inter-latin-wght-normal.woff2', to: 'wwwroot/lib/fonts/inter-latin-wght-normal.woff2' },
  { from: 'node_modules/@fontsource-variable/inter/files/inter-latin-ext-wght-normal.woff2', to: 'wwwroot/lib/fonts/inter-latin-ext-wght-normal.woff2' },
  ...['400', '700'].flatMap((w) => ['latin', 'latin-ext'].map((s) => ({
    from: `node_modules/@fontsource/space-mono/files/space-mono-${s}-${w}-normal.woff2`,
    to: `wwwroot/lib/fonts/space-mono-${s}-${w}-normal.woff2`,
  }))),

  // Font Awesome Free (CSS + woff2 webfonty) – nahrazuje externí kit.fontawesome.com
  { from: 'node_modules/@fortawesome/fontawesome-free/css/all.min.css', to: 'wwwroot/lib/fontawesome/css/all.min.css' },
  ...['fa-solid-900', 'fa-regular-400', 'fa-brands-400', 'fa-v4compatibility'].map((f) => ({
    from: `node_modules/@fortawesome/fontawesome-free/webfonts/${f}.woff2`,
    to: `wwwroot/lib/fontawesome/webfonts/${f}.woff2`,
  })),
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
