document.addEventListener("DOMContentLoaded", function () {
    function refreshOrderStatus() {
        fetch(window.location.href)
            .then(response => response.text())
            .then(html => {
                document.body.innerHTML = html;
            });
    }

    setInterval(refreshOrderStatus, 5000); // Refresh every 5 seconds
});
