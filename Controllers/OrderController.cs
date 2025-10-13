using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualQueueSystem.Data;
using VirtualQueueSystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualQueueSystem.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Place a New Order
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(int userId, string items)
        {
            var lastQueueNumber = await _context.Orders
                .OrderByDescending(o => o.QueueNumber)
                .Select(o => o.QueueNumber)
                .FirstOrDefaultAsync();

            var newOrder = new Order
            {
                UserId = userId,
                Items = items,
                QueueNumber = lastQueueNumber + 1,
                Status = "Pending"
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewQueue", new { userId });
        }

        // ✅ Show User Queue Status
        public async Task<IActionResult> ViewQueue(int userId)
        {
            var userOrders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderBy(o => o.QueueNumber)
                .ToListAsync();

            return View(userOrders);
        }

        // ✅ Admin: View All Orders (Kitchen Screen)
        public async Task<IActionResult> AdminOrders()
        {
            var orders = await _context.Orders
                .OrderBy(o => o.QueueNumber)
                .ToListAsync();

            return View(orders);
        }

        // ✅ Admin: Update Order Status
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();

            order.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction("AdminOrders");
        }
    }
}
