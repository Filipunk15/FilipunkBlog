/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Components/**/*.{razor,razor.cs,html,cshtml}",
    "./wwwroot/**/*.html",
    // ViewModely v Application vrací Tailwind třídy jako řetězce (indikátor dostupnosti apod.)
    "../FilipunkBlog.Application/**/*.cs",
  ],
  safelist: [
    // dynamicky skládané třídy stavu dostupnosti (Available / Busy / Unavailable)
    { pattern: /^(bg|text|border)-(emerald|amber|gray)-(400|500|600)(\/(5|10|40|50))?$/ },
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: ['Inter', 'ui-sans-serif', 'system-ui', '-apple-system', 'Segoe UI', 'sans-serif'],
        mono: ['"Space Mono"', 'ui-monospace', 'SFMono-Regular', 'Menlo', 'Consolas', 'monospace'],
      },
      typography: ({ theme }) => ({
        invert: {
          css: {
            '--tw-prose-invert-links': theme('colors.blue.500'),
            '--tw-prose-invert-bullets': theme('colors.gray.600'),
            '--tw-prose-invert-hr': theme('colors.gray.800'),
            '--tw-prose-invert-quote-borders': theme('colors.blue.600'),
            '--tw-prose-invert-pre-bg': 'transparent',
            '--tw-prose-invert-pre-code': theme('colors.gray.200'),
          },
        },
      }),
    },
  },
  plugins: [require('@tailwindcss/typography')],
};
