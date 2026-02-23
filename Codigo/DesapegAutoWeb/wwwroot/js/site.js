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
                window.location.href = '/';
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

    // ── CNPJ mask: .cnpj-mask ──────────────────────────────────────────────────
    function maskCnpj(value) {
        var digits = value.replace(/\D/g, "").slice(0, 14);
        if (digits.length <= 2) return digits;
        if (digits.length <= 5) return digits.slice(0, 2) + "." + digits.slice(2);
        if (digits.length <= 8) return digits.slice(0, 2) + "." + digits.slice(2, 5) + "." + digits.slice(5);
        if (digits.length <= 12) return digits.slice(0, 2) + "." + digits.slice(2, 5) + "." + digits.slice(5, 8) + "/" + digits.slice(8);
        return digits.slice(0, 2) + "." + digits.slice(2, 5) + "." + digits.slice(5, 8) + "/" + digits.slice(8, 12) + "-" + digits.slice(12);
    }

    document.querySelectorAll(".cnpj-mask").forEach(function (input) {
        // Format existing value on load
        if (input.value) input.value = maskCnpj(input.value);
        input.addEventListener("input", function () {
            this.value = maskCnpj(this.value);
        });
        input.addEventListener("paste", function (event) {
            event.preventDefault();
            var text = (event.clipboardData || window.clipboardData).getData("text");
            this.value = maskCnpj(text);
        });
    });

    // ── Telefone mask (BR): .telefone-mask ────────────────────────────────────
    function maskTelefone(value) {
        var digits = value.replace(/\D/g, "").slice(0, 11);
        var len = digits.length;
        if (len === 0) return "";
        if (len <= 2) return "(" + digits;
        if (len <= 6) return "(" + digits.slice(0, 2) + ") " + digits.slice(2);
        if (len <= 10) {
            // Landline: (XX) XXXX-XXXX
            return "(" + digits.slice(0, 2) + ") " + digits.slice(2, 6) + "-" + digits.slice(6);
        }
        // Cell: (XX) XXXXX-XXXX
        return "(" + digits.slice(0, 2) + ") " + digits.slice(2, 7) + "-" + digits.slice(7);
    }

    document.querySelectorAll(".telefone-mask").forEach(function (input) {
        // Format existing value on load
        if (input.value) input.value = maskTelefone(input.value);
        input.addEventListener("input", function () {
            var cursor = this.selectionStart;
            var prevLen = this.value.length;
            this.value = maskTelefone(this.value);
            // Restore cursor approximately
            var diff = this.value.length - prevLen;
            this.setSelectionRange(cursor + diff, cursor + diff);
        });
        input.addEventListener("keydown", function (e) {
            // Only allow digits, backspace, delete, tab, arrows, ctrl/meta combos
            if ([8, 9, 13, 27, 35, 36, 37, 38, 39, 40, 46].indexOf(e.keyCode) !== -1) return;
            if (e.ctrlKey || e.metaKey) return;
            if (e.keyCode >= 48 && e.keyCode <= 57) return;   // 0–9 top row
            if (e.keyCode >= 96 && e.keyCode <= 105) return;  // 0–9 numpad
            e.preventDefault();
        });
        input.addEventListener("paste", function (event) {
            event.preventDefault();
            var text = (event.clipboardData || window.clipboardData).getData("text");
            this.value = maskTelefone(text);
        });
    });

    // ── Ano field validation: .ano-field ──────────────────────────────────────
    document.querySelectorAll(".ano-field").forEach(function (input) {
        var errorEl = document.getElementById("ano-error-msg");

        function getAnoMsg(value) {
            if (!value) return "";
            var year = parseInt(value, 10);
            var maxYear = new Date().getFullYear() + 1;
            if (value.length < 4) return "Digite um ano com 4 dígitos.";
            if (year < 1886) return "O automóvel foi inventado em 1886. Insira um ano a partir de 1886.";
            if (year > maxYear) return "O ano máximo permitido é " + maxYear + " (ano atual + 1).";
            return "";
        }

        function updateAnoError() {
            var msg = getAnoMsg(input.value);
            input.setCustomValidity(msg);
            if (errorEl) {
                errorEl.textContent = msg;
                errorEl.style.display = msg ? "" : "none";
            }
        }

        input.addEventListener("input", function () {
            // Strip non-digits, limit to 4 chars
            this.value = this.value.replace(/\D/g, "").slice(0, 4);
            updateAnoError();
        });

        input.addEventListener("blur", updateAnoError);

        // Validate on load if field already has a value
        if (input.value) updateAnoError();
    });

    // ── Preço field mask: .preco-field ────────────────────────────────────────
    document.querySelectorAll(".preco-field").forEach(function (input) {
        // On load, if value contains a period (from model), show comma instead
        if (input.value && input.value.indexOf(".") !== -1) {
            input.value = input.value.replace(".", ",");
        }

        input.addEventListener("keydown", function (e) {
            // Allow: Backspace, Delete, Tab, Escape, Enter, arrows, Home, End
            if ([8, 9, 13, 27, 35, 36, 37, 38, 39, 40, 46].indexOf(e.keyCode) !== -1) return;
            if (e.ctrlKey || e.metaKey) return;
            // Allow digits 0-9
            if (e.keyCode >= 48 && e.keyCode <= 57) return;
            if (e.keyCode >= 96 && e.keyCode <= 105) return;
            // Allow comma (key 188) — only if no comma yet
            if (e.keyCode === 188 || e.keyCode === 110) {
                if (this.value.indexOf(",") !== -1) {
                    e.preventDefault(); // already has a comma
                }
                return;
            }
            e.preventDefault();
        });

        input.addEventListener("input", function () {
            var val = this.value;
            // Strip everything except digits and comma
            val = val.replace(/[^0-9,]/g, "");
            // Only one comma allowed
            var firstComma = val.indexOf(",");
            if (firstComma !== -1) {
                var before = val.slice(0, firstComma).replace(/,/g, "");
                var after  = val.slice(firstComma + 1).replace(/,/g, "").slice(0, 2);
                val = before + "," + after;
            }
            this.value = val;
        });
    });

    // Before any form submit, normalize preco comma → period for backend parsing
    document.querySelectorAll("form").forEach(function (form) {
        form.addEventListener("submit", function () {
            form.querySelectorAll(".preco-field").forEach(function (input) {
                input.value = input.value.replace(",", ".");
            });
        });
    });

    // Clean URL - remove empty query params on form submit (GET only)
    document.querySelectorAll("form[method='get']").forEach(function (form) {
        form.addEventListener("submit", function (e) {
            const inputs = form.querySelectorAll("input, select, textarea");
            inputs.forEach(input => {
                if (!input.value || input.value === "") {
                    input.disabled = true; // Disable empty inputs so they aren't sent
                }
            });
            
            // Re-enable after a short delay so the user doesn't see disabled fields if they stay on page
            setTimeout(() => {
                inputs.forEach(input => input.disabled = false);
            }, 100);
        });
    });
})();
