// Small TOC enhancements: toggle on mobile, smooth scrolling, active link highlight
document.addEventListener("DOMContentLoaded", function () {
    // Smooth-scrolling is handled by CSS `scroll-behavior`, but we'll ensure offset-friendly scroll
    try {
        document.documentElement.style.scrollPaddingTop = "80px";
    } catch (e) {}

    // Add toggle button for mobile TOC containers
    document.querySelectorAll(".toc-container").forEach(function (toc) {
        var toggle = document.createElement("button");
        toggle.className = "toc-toggle";
        toggle.type = "button";
        toggle.setAttribute("aria-expanded", "false");
        toggle.textContent = "Contents";

        toggle.addEventListener("click", function () {
            var expanded = toggle.getAttribute("aria-expanded") === "true";
            toggle.setAttribute("aria-expanded", String(!expanded));
            toc.classList.toggle("expanded", !expanded);
            toc.classList.toggle("collapsed", expanded);
        });

        toc.insertBefore(toggle, toc.firstChild);
    });

    // Smooth scroll for internal anchor links within TOC
    document.body.addEventListener(
        "click",
        function (e) {
            var target = e.target;
            if (
                target.tagName === "A" &&
                target.getAttribute("href") &&
                target.getAttribute("href").startsWith("#")
            ) {
                var id = target.getAttribute("href").slice(1);
                var el = document.getElementById(id);
                if (el) {
                    e.preventDefault();
                    el.scrollIntoView({ behavior: "smooth", block: "start" });
                    history.replaceState(null, "", "#" + id);
                }
            }
        },
        true,
    );
});
