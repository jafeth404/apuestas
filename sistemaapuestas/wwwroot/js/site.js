// Dark mode toggle
document.addEventListener('DOMContentLoaded', function () {
    const html = document.getElementById('htmlRoot');
    const btn = document.getElementById('darkModeToggle');
    const icon = document.getElementById('darkModeIcon');

    if (!btn) return;

    function applyTheme(isDark) {
        if (isDark) {
            html.setAttribute('data-bs-theme', 'dark');
            icon.classList.replace('bi-moon-fill', 'bi-sun-fill');
        } else {
            html.removeAttribute('data-bs-theme');
            icon.classList.replace('bi-sun-fill', 'bi-moon-fill');
        }
    }

    applyTheme(html.getAttribute('data-bs-theme') === 'dark');

    btn.addEventListener('click', function () {
        const isDark = html.getAttribute('data-bs-theme') !== 'dark';
        applyTheme(isDark);
        localStorage.setItem('theme', isDark ? 'dark' : 'light');
        document.dispatchEvent(new CustomEvent('themeChanged', { detail: { dark: isDark } }));
    });
});
