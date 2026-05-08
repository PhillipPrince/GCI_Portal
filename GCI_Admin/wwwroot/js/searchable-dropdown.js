document.addEventListener("DOMContentLoaded", function () {

    document.querySelectorAll(".searchable-dropdown").forEach(container => {

        const input = container.querySelector(".search-input");
        const hidden = container.querySelector(".selected-value");
        const dropdown = container.querySelector(".dropdown-list");

        const items = JSON.parse(container.dataset.items || "[]");

        function render(list) {
            dropdown.innerHTML = "";

            if (!list.length) {
                dropdown.style.display = "none";
                return;
            }

            list.forEach(i => {

                const btn = document.createElement("button");
                btn.type = "button";
                btn.className = "list-group-item list-group-item-action";

                btn.innerHTML = `
                    <strong>${i.text}</strong><br/>
                    <small>${i.extra1 || ""} | ${i.extra2 || ""}</small>
                `;

                btn.onclick = function () {
                    hidden.value = i.value;
                    input.value = i.text;
                    dropdown.style.display = "none";
                };

                dropdown.appendChild(btn);
            });

            dropdown.style.display = "block";
        }

        input.addEventListener("input", function () {

            const q = this.value.toLowerCase();

            if (!q) {
                dropdown.style.display = "none";
                return;
            }

            const filtered = items.filter(i =>
                (i.text || "").toLowerCase().includes(q)
            );

            render(filtered);
        });

        document.addEventListener("click", function (e) {
            if (!container.contains(e.target)) {
                dropdown.style.display = "none";
            }
        });
    });
});