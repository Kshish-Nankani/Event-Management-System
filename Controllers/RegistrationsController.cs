using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Controllers
{
    public class RegistrationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegistrationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------- USER: My Registrations ----------
        public IActionResult MyRegistrations()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var registrations = _context.Registrations
                .Include(r => r.Event)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RegisteredOn)
                .ToList();

            return View(registrations);
        }

        // ---------- ADMIN: All Registrations ----------
        public IActionResult AllRegistrations()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized();

            var registrations = _context.Registrations
                .Include(r => r.Event)
                .Include(r => r.User)
                .OrderByDescending(r => r.RegisteredOn)
                .ToList();

            return View(registrations);
        }

        // ---------- REGISTER FOR EVENT ----------
        public IActionResult RegisterEvent(int eventId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Check if already registered
            bool exists = _context.Registrations
                .Any(r => r.EventId == eventId && r.UserId == userId);

            if (exists)
            {
                TempData["ErrorMessage"] = "You are already registered for this event.";
                return RedirectToAction("Index", "Events");
            }

            var registration = new Models.Registration
            {
                EventId = eventId,
                UserId = userId.Value,
                RegisteredOn = DateTime.Now
            };

            _context.Registrations.Add(registration);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Registered successfully!";
            return RedirectToAction("Index", "Events");
        }
    }
}
