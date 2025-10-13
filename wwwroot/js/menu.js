document.addEventListener("DOMContentLoaded", function () {
    console.log("✅ menu.js Loaded Successfully!");

    // 🛒 Cart Data
    let cartItems = [];
    const cartList = document.getElementById("cart-items");
    const totalPriceElement = document.getElementById("total-price");
    const checkoutButton = document.getElementById("checkout-btn");

    // 🍽️ Category Data
    const categoryButtons = document.querySelectorAll(".category-btn");
    const allMenuItems = document.querySelectorAll(".menu-card");

    // ❗ Ensure checkout button exists
    if (!checkoutButton) {
        console.error("❌ ERROR: Checkout button not found! Check your HTML.");
        return;
    }

    console.log("✅ Checkout button found!");

    // ✅ Function to Update Cart UI
    function updateCart() {
        cartList.innerHTML = "";
        let totalPrice = 0;

        cartItems.forEach((item, index) => {
            const listItem = document.createElement("li");
            listItem.innerHTML = `
                <span>${item.name} - $${item.price.toFixed(2)}</span>
                <button class="remove-btn" data-index="${index}">❌</button>
            `;
            cartList.appendChild(listItem);
            totalPrice += item.price;
        });

        totalPriceElement.textContent = `$${totalPrice.toFixed(2)}`;

        // Enable scrolling if cart items overflow
        cartList.style.overflowY = cartItems.length > 5 ? "auto" : "hidden";
    }

    // ✅ Add to Cart Functionality
    document.querySelectorAll(".add-to-cart").forEach(button => {
        button.addEventListener("click", function () {
            const itemName = this.getAttribute("data-name");
            const itemPrice = parseFloat(this.getAttribute("data-price"));

            cartItems.push({ name: itemName, price: itemPrice });
            console.log(`🛒 Added to Cart: ${itemName} ($${itemPrice.toFixed(2)})`);
            updateCart();
        });
    });

    // ✅ Remove Item from Cart
    cartList.addEventListener("click", function (event) {
        if (event.target.classList.contains("remove-btn")) {
            const index = event.target.getAttribute("data-index");
            console.log(`🗑️ Removing item at index: ${index}`);
            cartItems.splice(index, 1);
            updateCart();
        }
    });

    // ✅ Category Filtering
    categoryButtons.forEach(button => {
        button.addEventListener("click", function () {
            const selectedCategory = this.getAttribute("data-category");
            console.log(`📂 Filtering Category: ${selectedCategory}`);

            // Remove "active" class from all buttons
            categoryButtons.forEach(btn => btn.classList.remove("active"));
            this.classList.add("active");

            // Show/Hide menu items based on category
            allMenuItems.forEach(item => {
                if (selectedCategory === "all" || item.getAttribute("data-category") === selectedCategory) {
                    item.style.display = "block";
                } else {
                    item.style.display = "none";
                }
            });
        });
    });

    // ✅ Checkout Functionality
    checkoutButton.addEventListener("click", function () {
        console.log("🛍️ Checkout button clicked!");

        if (cartItems.length === 0) {
            alert("⚠️ Your cart is empty!");
            return;
        }

        let orderDetails = cartItems.map(item => `${item.name} - $${item.price.toFixed(2)}`).join("\n");
        
         // Send the order request to the backend
    fetch("http://localhost:5000/api/Order/Create", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            "Items": cartItems,
            "Total": totalPriceElement.textContent.replace("$", "")
        })
    })
    .then(response => response.json())
    .then(data => {
        console.log("Order Placed:", data);
        
        alert(`✅ Your order has been placed!\n\n🛍️ Order Details:\n${orderDetails}\n\n💳 Total: ${totalPriceElement.textContent}`);

          // Redirect user to queue page
          window.location.href = "/User/Queue";
        })
        .catch(error => console.error("Error:", error));
        // Clear the cart after checkout
        cartItems = [];
        updateCart();
    });

    // ✅ Apply Styling to Checkout Button
    checkoutButton.style.background = "#007bff";
    checkoutButton.style.color = "white";
    checkoutButton.style.padding = "12px";
    checkoutButton.style.borderRadius = "8px";
    checkoutButton.style.fontSize = "16px";
    checkoutButton.style.border = "none";
    checkoutButton.style.cursor = "pointer";
    checkoutButton.style.width = "100%";
    checkoutButton.style.textAlign = "center";

    checkoutButton.addEventListener("mouseover", function () {
        checkoutButton.style.background = "#0056b3";
    });

    checkoutButton.addEventListener("mouseout", function () {
        checkoutButton.style.background = "#007bff";
    });

    console.log("✅ All Event Listeners Added Successfully!");
});
