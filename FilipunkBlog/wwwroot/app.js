// Theme toggle
const THEME_KEY = 'filipunk-theme';

function initTheme() {
    const saved = localStorage.getItem(THEME_KEY);
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    const isDark = saved ? saved === 'dark' : prefersDark;
    document.documentElement.classList.toggle('dark', isDark);
}

function toggleTheme() {
    const isDark = document.documentElement.classList.toggle('dark');
    localStorage.setItem(THEME_KEY, isDark ? 'dark' : 'light');
}

initTheme();