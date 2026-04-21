(function () {
    const menuButton = document.getElementById("mobileMenuToggle");
    const menuGlyph = document.getElementById("mobileMenuToggleGlyph");
    const drawer = document.getElementById("mobileNavDrawer");
    const backdrop = document.getElementById("mobileNavBackdrop");

    if (!menuButton || !drawer || !backdrop) {
        return;
    }

    function setOpenState(isOpen) {
        if (isOpen) {
            drawer.classList.add("is-open");
            backdrop.hidden = false;
            backdrop.classList.add("is-open");
            menuButton.setAttribute("aria-expanded", "true");
            menuButton.setAttribute("aria-label", "Close navigation menu");
            drawer.setAttribute("aria-hidden", "false");
            document.body.classList.add("mobile-nav-open");
            if (menuGlyph) {
                menuGlyph.textContent = "X";
            }
            return;
        }

        drawer.classList.remove("is-open");
        backdrop.classList.remove("is-open");
        backdrop.hidden = true;
        menuButton.setAttribute("aria-expanded", "false");
        menuButton.setAttribute("aria-label", "Open navigation menu");
        drawer.setAttribute("aria-hidden", "true");
        document.body.classList.remove("mobile-nav-open");
        if (menuGlyph) {
            menuGlyph.textContent = "≡";
        }
    }

    menuButton.addEventListener("click", function () {
        const isOpen = drawer.classList.contains("is-open");
        setOpenState(!isOpen);
    });

    backdrop.addEventListener("click", function () {
        setOpenState(false);
    });

    document.querySelectorAll("[data-mobile-nav-close='true']").forEach(function (element) {
        element.addEventListener("click", function () {
            setOpenState(false);
        });
    });

    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape") {
            setOpenState(false);
        }
    });
})();
