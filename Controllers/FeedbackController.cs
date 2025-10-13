using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VirtualQueueSystem.Data;
using VirtualQueueSystem.Models;
using System.Collections.Generic;

namespace VirtualQueueSystem.Controllers
{
    [Authorize] // ✅ Only logged-in users can access feedback
    public class FeedbackController : Controller
    {
        private readonly AppDbContext _context;

        public FeedbackController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Corrected Route: This ensures the method is accessible
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var feedbacks = await _context.Feedbacks.Include(f => f.User).ToListAsync();
            return View("~/Views/User/Feedback.cshtml", feedbacks ?? new List<Feedback>()); // ✅ Fix View Path
        }

        // ✅ Corrected Route for Submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.UserId = GetCurrentUserId();
                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "✅ Thank you for your feedback!";
                return RedirectToAction("Index");
            }

            return View("~/Views/User/Feedback.cshtml", feedback);
        }

        // ✅ Get Current User ID
        private int GetCurrentUserId()
        {
            if (User.Identity?.IsAuthenticated != true)
                return 0;

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out int userId) ? userId : 0;
        }
    }
}
