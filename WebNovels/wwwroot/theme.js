window.themeFunctions = {
    applyTheme: function () {
        const storedTheme = localStorage.getItem('theme');
        if (storedTheme === 'dark') {
            document.documentElement.classList.add('dark-mode');
            return true;
        } else {
            document.documentElement.classList.remove('dark-mode');
            return false;
        }
    },
    toggleTheme: function () {
        const isDark = document.documentElement.classList.toggle('dark-mode');
        localStorage.setItem('theme', isDark ? 'dark' : 'light');
        return isDark;
    },
    getCurrentTheme: function () {
        return localStorage.getItem('theme') || 'light';
    }
};

document.addEventListener("DOMContentLoaded", () => {
    window.themeFunctions.applyTheme();
    const observer = new MutationObserver(() => {
        window.themeFunctions.applyTheme();
    });

    observer.observe(document.body, {
        childList: true,
        subtree: true
    });
});
