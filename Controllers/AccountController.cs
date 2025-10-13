using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization; // ✅ Fixed Missing Namespace
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VirtualQueueSystem.Models;
using VirtualQueueSystem.Data;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualQueueSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        // ✅ Pre-approved admin accounts (Hardcoded for security)
        private static readonly List<string> ApprovedAdmins = new List<string>
        {
            "admin1", "admin2"
        };

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "⚠️ Username and password are required.";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
            {
                ViewBag.Error = "❌ Invalid username or password.";
                return View();
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) // ✅ Fixed: Use `PasswordHash`
            {
                ViewBag.Error = "❌ Invalid username or password.";
                return View();
            }

            if (user.Role == "Admin" && !ApprovedAdmins.Contains(user.Username.ToLower()))
            {
                ViewBag.Error = "❌ Unauthorized admin login attempt.";
                return View();
            }

            // ✅ Create Authentication Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return user.Role == "Admin"
                ? RedirectToAction("Dashboard", "Admin")
                : RedirectToAction("Dashboard", "User");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.PasswordHash)) // ✅ Fixed to check `PasswordHash`
            {
                ViewBag.Error = "⚠️ Username and password are required.";
                return View(user);
            }

            if (_context.Users.Any(u => u.Username.ToLower() == user.Username.ToLower()))
            {
                ViewBag.Error = "❌ Username already exists!";
                return View(user);
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash); // ✅ Fixed: Use `PasswordHash`
            user.Role = "User"; // ✅ Only users can register

            _context.Users.Add(user);
            _context.SaveChanges();
            
            return RedirectToAction("Login");
        }

        [Authorize] // ✅ Fixed: `Authorize` attribute now works
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
