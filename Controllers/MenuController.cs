using Microsoft.AspNetCore.Mvc;
using VirtualQueueSystem.Data;
using VirtualQueueSystem.Models;
using System.Linq;

namespace VirtualQueueSystem.Controllers
{
    public class MenuController : Controller
    {
        private readonly AppDbContext _context;

        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var menuItems = _context.MenuItems.ToList();
            return View(menuItems);
        }
    }
}