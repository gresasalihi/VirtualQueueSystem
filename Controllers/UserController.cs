using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using VirtualQueueSystem.Data;
using VirtualQueueSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualQueueSystem.Controllers
{
    [Authorize] // ✅ Ensures only logged-in users can access this controller
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ User Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }

        // ✅ Menu Page
        public async Task<IActionResult> Menu()
        {
            var menuItems = await _context.MenuItems.ToListAsync();
            return View(menuItems);
        }

        // ✅ Add Item to Cart (Session-Based)
        [HttpPost]
        public IActionResult AddToCart(int itemId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<int>>("Cart") ?? new List<int>();
            if (!cart.Contains(itemId)) // Prevent duplicate items
            {
                cart.Add(itemId);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                TempData["CartMessage"] = "✅ Item added to cart!";
            }
            else
            {
                TempData["CartMessage"] = "⚠️ Item is already in the cart!";
            }

            return RedirectToAction("Menu");
        }

        // ✅ Checkout: Process Order & Assign Queue Number
        public async Task<IActionResult> Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<int>>("Cart") ?? new List<int>();
            if (!cart.Any())
            {
                TempData["CartMessage"] = "⚠️ Your cart is empty!";
                return RedirectToAction("Menu");
            }

            var items = await _context.MenuItems.Where(m => cart.Contains(m.Id)).ToListAsync();

            // ✅ Assign the Next Available Queue Number
            var lastQueueNumber = await _context.Orders
                .OrderByDescending(o => o.QueueNumber)
                .Select(o => o.QueueNumber)
                .FirstOrDefaultAsync();

            var newOrder = new Order
            {
                UserId = GetCurrentUserId(),
                Items = string.Join(", ", items.Select(i => i.Name)), // Store item names as a string
                QueueNumber = lastQueueNumber + 1,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("Cart"); // ✅ Clear cart after order

            TempData["SuccessMessage"] = $"✅ Order placed! Queue Number: {newOrder.QueueNumber}";
            return RedirectToAction("ViewQueue");
        }

        // ✅ View Queue with Status Updates
        public async Task<IActionResult> ViewQueue()
        {
            int userId = GetCurrentUserId();
            var userOrders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderBy(o => o.QueueNumber)
                .ToListAsync();

            return View("ViewQueue", userOrders);
        }

        // ✅ Cancel Order (Only if Pending)
        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order != null && order.Status == "Pending")
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "✅ Order canceled successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "⚠️ Order not found or cannot be canceled.";
            }

            return RedirectToAction("ViewQueue");
        }

        // ✅ Feedback Page
        public async Task<IActionResult> Feedback()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.User)
                .ToListAsync();

            return View("Feedback", feedbacks ?? new List<Feedback>()); // ✅ Fix View Name
        }

        // ✅ Submit Feedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.UserId = GetCurrentUserId();
                feedback.CreatedAt = DateTime.Now; // ✅ Ensure timestamp is set
                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "✅ Thank you for your feedback!";
                return RedirectToAction("Feedback");
            }

            return View("Feedback", feedback);
        }

        // ✅ Get Current User ID Safely
        private int GetCurrentUserId()
        {
            if (User.Identity?.IsAuthenticated != true)
                return 0;

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }
    }
}

// ✅ Extension methods for session handling (Must be outside the controller)
public static class SessionExtensions
{
    public static void SetObjectAsJson(this ISession session, string key, object value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static T? GetObjectFromJson<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}
