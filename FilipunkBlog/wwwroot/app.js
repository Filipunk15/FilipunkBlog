console.log("app.js loaded");

window.goHome = () => {
    if (window.location.pathname === '/') {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    } else {
        window.location.href = '/';
    }
}