// DesapegAuto — shared behaviors

(function () {
    "use strict";

    // Back button
    function initBackBtn() {
        const header = document.querySelector('.da-topbar[data-controller]');
        const btn = document.querySelector('.da-back-btn');
        if (!btn || !header) return;

        const controller = (header.dataset.controller || '').toLowerCase();
        const action = (header.dataset.action || '').toLowerCase();

        if (controller === 'home' && action === 'index') {
            btn.classList.add('is-hidden');
        } else {
            btn.addEventListener('click', function () {
                history.back();
            });
        }
    }
    initBackBtn();

    // Theme Toggle (Animated Switch)
    function initThemeToggle() {
        const switches = document.querySelectorAll('.da-theme-switch');
        if (!switches.length) return;

        switches.forEach(sw => {
            sw.addEventListener('click', () => {
                const currentTheme = document.documentElement.getAttribute('data-theme') || 'light';
                const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
                
                document.documentElement.setAttribute('data-theme', newTheme);
                localStorage.setItem('da-theme', newTheme);
            });
        });
    }
    initThemeToggle();

    // Tab switching: .da-tab-btn[data-tab] / .da-tab-panel
    document.querySelectorAll(".da-tab-btn[data-tab]").forEach(function (button) {
        button.addEventListener("click", function () {
            document.querySelectorAll(".da-tab-btn").forEach(function (btn) {
                btn.classList.remove("is-active");
            });
            document.querySelectorAll(".da-tab-panel").forEach(function (panel) {
                panel.classList.remove("is-active");
            });
            button.classList.add("is-active");
            var panel = document.getElementById("tab-" + button.dataset.tab);
            if (panel) panel.classList.add("is-active");
        });
    });

    // Switch panels: .da-switch-btn[data-switch] / .da-switch-panel
    document.querySelectorAll(".da-switch-btn[data-switch]").forEach(function (button) {
        button.addEventListener("click", function () {
            document.querySelectorAll(".da-switch-btn").forEach(function (btn) {
                btn.classList.remove("is-active");
            });
            document.querySelectorAll(".da-switch-panel").forEach(function (panel) {
                panel.classList.remove("is-active");
            });
            button.classList.add("is-active");
            var panel = document.getElementById("switch-" + button.dataset.switch);
            if (panel) panel.classList.add("is-active");
        });
    });

    // CNPJ mask: .cnpj-mask
    function maskCnpj(value) {
        var digits = value.replace(/\D/g, "").slice(0, 14);
        if (digits.length <= 2) return digits;
        if (digits.length <= 5) return digits.slice(0, 2) + "." + digits.slice(2);
        if (digits.length <= 8) return digits.slice(0, 2) + "." + digits.slice(2, 5) + "." + digits.slice(5);
        if (digits.length <= 12) return digits.slice(0, 2) + "." + digits.slice(2, 5) + "." + digits.slice(5, 8) + "/" + digits.slice(8);
        return digits.slice(0, 2) + "." + digits.slice(2, 5) + "." + digits.slice(5, 8) + "/" + digits.slice(8, 12) + "-" + digits.slice(12);
    }

    document.querySelectorAll(".cnpj-mask").forEach(function (input) {
        input.addEventListener("input", function () {
            this.value = maskCnpj(this.value);
        });
        input.addEventListener("paste", function (event) {
            event.preventDefault();
            var text = (event.clipboardData || window.clipboardData).getData("text");
            this.value = maskCnpj(text);
        });
    });
})();
