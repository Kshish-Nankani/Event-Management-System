using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.ViewModels;

namespace EventManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------- REGISTER ----------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Check if email already exists
            bool exists = _context.Users.Any(x => x.Email == vm.Email);
            if (exists)
            {
                ModelState.AddModelError("", "Email already exists");
                return View(vm);
            }

            // Create user
            var user = new User
            {
                FullName = vm.FullName,
                Email = vm.Email,
                Password = vm.Password,
                Role = "User"   // default role
            };

            // ✅ Assign Admin role based on email
            if (vm.Email.ToLower() == "admin@example.com") // Change email as needed
            {
                user.Role = "Admin";
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // ---------- LOGIN ----------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Find user by email & password
            var user = _context.Users
                .FirstOrDefault(x => x.Email == vm.Email && x.Password == vm.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Email or Password");
                return View(vm);
            }

            // ✅ Set session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("Role", user.Role);

            // Redirect to Events index
            return RedirectToAction("Index", "Events");
        }

        // ---------- LOGOUT ----------
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
