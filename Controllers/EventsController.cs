using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------- VIEW ALL EVENTS ----------
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            var events = _context.Events
                .OrderByDescending(e => e.EventDate)
                .ToList();

            return View(events);
        }

        // ---------- CREATE EVENT (ADMIN) ----------
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized();

            return View();
        }

        [HttpPost]
        public IActionResult Create(EventVM vm)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized();

            if (!ModelState.IsValid)
                return View(vm);

            var ev = new Event
            {
                Title = vm.Title,
                Description = vm.Description,
                EventDate = vm.EventDate,
                Location = vm.Location,
                CreatedBy = HttpContext.Session.GetInt32("UserId").Value
            };

            _context.Events.Add(ev);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Event created successfully!";
            return RedirectToAction("Index");
        }

        // ---------- EDIT EVENT (ADMIN) ----------
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized();

            var ev = _context.Events.Find(id);
            if (ev == null)
                return NotFound();

            return View(ev);
        }

        [HttpPost]
        public IActionResult Edit(Event ev)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized();

            if (!ModelState.IsValid)
                return View(ev);

            _context.Events.Update(ev);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Event updated successfully!";
            return RedirectToAction("Index");
        }

        // ---------- DELETE EVENT (ADMIN) ----------
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized();

            var ev = _context.Events.Find(id);
            if (ev == null)
                return NotFound();

            _context.Events.Remove(ev);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Event deleted successfully!";
            return RedirectToAction("Index");
        }

        // ---------- VIEW SINGLE EVENT DETAILS (Optional) ----------
        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            var ev = _context.Events
                .FirstOrDefault(e => e.Id == id);

            if (ev == null)
                return NotFound();

            return View(ev);
        }
    }
}
