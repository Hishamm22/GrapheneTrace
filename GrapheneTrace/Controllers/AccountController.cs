using System.Threading.Tasks;
using GrapheneTrace.Data;
using GrapheneTrace.Models;
using Microsoft.AspNetCore.Http;
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
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Show validation errors
                return View(model);
            }

            // Look up the user by email + password (PasswordHash stores plain text for the assignment)
            var user = await _context.Users
                .Include(u => u.PatientProfile)
                .FirstOrDefaultAsync(u =>
                    u.Email == model.Email &&
                    u.PasswordHash == model.Password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View(model);
            }

            // Store who is logged in in session
            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("UserName", user.FullName ?? user.Email);
            HttpContext.Session.SetString("UserRole", user.Role ?? "Patient");

            // Send patients to their dashboard
            var role = (user.Role ?? "").ToLowerInvariant();
            if (role == "patient" || user.PatientProfile != null)
            {
                return RedirectToAction("Dashboard", "Patient");
            }

            if (role == "admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            if (role == "clinician")
            {
                return RedirectToAction("Dashboard", "Clinician");
            }

            // Fallback
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
