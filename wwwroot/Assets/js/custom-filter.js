document.addEventListener("DOMContentLoaded", function () {

    const headers = document.querySelectorAll(".filterable-header");

    headers.forEach(header => {
        header.addEventListener("click", function (event) {
            if (event.target.closest(".filter-dropdown")) return;
            event.stopPropagation();

            const dropdown = this.querySelector('.filter-dropdown');
            if (!dropdown) return;

            const isOpening = !dropdown.classList.contains("show");

            closeAllFilters();

            if (isOpening) {
                positionDropdown(this, dropdown);
                dropdown.classList.add("show");
            }
        });
    });

    const options = document.querySelectorAll(".filter-option");
    options.forEach(option => {
        option.addEventListener("click", function (event) {
            event.stopPropagation();

            const header = this.closest(".filterable-header");
            const filterName = header.dataset.filter;
            const filterValue = this.dataset.value;

            applyFilter(filterName, filterValue);
            closeAllFilters();
        });
    });

    window.addEventListener("click", () => closeAllFilters());
    // Chỉ đóng khi scroll ngoài dropdown, không đóng khi scroll bên trong dropdown
    window.addEventListener('scroll', function (e) {
        const dropdown = document.querySelector(".filter-dropdown.show");
        if (dropdown && !dropdown.contains(e.target)) {
            closeAllFilters(null);
        }
    }, true);

    window.addEventListener("resize", () => closeAllFilters());
});

function positionDropdown(header, dropdown) {

    dropdown.style.visibility = "hidden";
    dropdown.style.opacity = "0";
    dropdown.style.pointerEvents = "none";
    dropdown.classList.add("measuring");

    const headerRect = header.getBoundingClientRect();
    const screenWidth = window.innerWidth;
    const screenHeight = window.innerHeight;

    dropdown.style.position = 'fixed';
    dropdown.style.top = `${headerRect.bottom}px`;
    dropdown.style.left = `${headerRect.left}px`;
    dropdown.style.minWidth = `${headerRect.width}px`;

    if (headerRect.left + dropdown.offsetWidth > screenWidth) {
        dropdown.style.left = 'auto';
        dropdown.style.right = `${screenWidth - headerRect.right}px`;
    }

    if (headerRect.bottom + dropdown.offsetHeight > screenHeight) {
        dropdown.style.top = `${headerRect.top - dropdown.offsetHeight}px`;
    }

    dropdown.classList.remove("measuring");
    dropdown.style.visibility = "";
    dropdown.style.opacity = "";
    dropdown.style.pointerEvents = "";
}

function closeAllFilters() {
    document.querySelectorAll(".filter-dropdown.show").forEach(d => d.classList.remove("show"));
}

function applyFilter(filterName, filterValue) {
    const url = new URL(window.location.href);
    if (filterValue) url.searchParams.set(filterName, filterValue);
    else url.searchParams.delete(filterName);
    url.searchParams.set("page", "1");
    window.location.href = url.toString();
}
