// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

(() => {
	const root = document.documentElement;
	const toggleButtons = document.querySelectorAll("[data-theme-toggle]");
	const iconElements = document.querySelectorAll("[data-theme-icon]");

	const getTheme = () => root.getAttribute("data-theme") === "dark" ? "dark" : "light";

	const applyTheme = (theme) => {
		root.setAttribute("data-theme", theme);
		const icon = theme === "dark" ? "☀️" : "🌙";
		const label = theme === "dark" ? "Alternar para tema claro" : "Alternar para tema escuro";

		iconElements.forEach((element) => {
			element.textContent = icon;
		});

		toggleButtons.forEach((button) => {
			button.setAttribute("aria-label", label);
			button.setAttribute("title", label);
		});
	};

	const saveTheme = (theme) => {
		try {
			localStorage.setItem("da-theme", theme);
		} catch {
			// Ignora bloqueios de storage no navegador.
		}
	};

	applyTheme(getTheme());

	toggleButtons.forEach((button) => {
		button.addEventListener("click", () => {
			const nextTheme = getTheme() === "dark" ? "light" : "dark";
			applyTheme(nextTheme);
			saveTheme(nextTheme);
		});
	});
})();
