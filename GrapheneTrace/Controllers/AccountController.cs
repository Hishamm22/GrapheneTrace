using System;
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
                return View(model);

            // simple plain-text password check (OK for coursework only)
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email == model.Email &&
                    u.PasswordHash == model.Password &&
                    u.AccountStatus == "Active");

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("UserRole", user.Role ?? "Patient");
            HttpContext.Session.SetString("UserName", user.FullName ?? user.Email);

            // for now all logins go to patient dashboard
            return RedirectToAction("Dashboard", "Patient");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (existing != null)
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View(model);
            }

            var user = new User
            {
                Email = model.Email,
                PasswordHash = model.Password,
                FullName = model.FullName,
                Role = "Patient",
                AccountStatus = "Active",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var patient = new Patient
            {
                UserID = user.UserID,
                DateOfBirth = model.DateOfBirth ?? new DateTime(1990, 1, 1),
                EmergencyContactName = model.EmergencyContactName ?? "Not set",
                EmergencyContactNumber = model.EmergencyContactNumber ?? "Not set",
                MedicalNotes = "Newly registered patient. No sensor data linked yet."
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            TempData["RegisterSuccess"] = "Account created successfully. You can now log in.";
            return RedirectToAction("Login");
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
