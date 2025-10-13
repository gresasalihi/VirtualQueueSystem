document.getElementById("checkout-btn").addEventListener("click", function () {
    let cartItems = JSON.parse(localStorage.getItem("cart")) || [];

    if (cartItems.length === 0) {
        alert("⚠️ Your cart is empty!");
    } else {
        let message = "🛒 Your order:\n\n";
        cartItems.forEach(item => {
            message += `✅ ${item.name} - $${item.price}\n`;
        });
        alert(message);
    }
});
