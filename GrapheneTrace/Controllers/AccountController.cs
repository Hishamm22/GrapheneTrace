using GrapheneTrace.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GrapheneTrace.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter both email and password.";
                return View();
            }

            // Simple login check: email + password. For the assignment this is OK.
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);

            if (user == null || user.AccountStatus != "Active")
            {
                ViewBag.Error = "Invalid credentials or inactive account.";
                return View();
            }

            // Store the logged-in user's details in session.
            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserName", user.FullName);

            // Redirect based on role.
            if (user.Role == "Patient")
            {
                return RedirectToAction("Dashboard", "Patient");
            }
            else if (user.Role == "Clinician")
            {
                // Clinician dashboard can be implemented later.
                return RedirectToAction("Dashboard", "Clinician");
            }

            // Default fall-back.
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
