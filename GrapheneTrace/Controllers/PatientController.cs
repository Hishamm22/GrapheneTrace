using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrapheneTrace.Data;
using GrapheneTrace.Models;
using GrapheneTrace.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GrapheneTrace.Controllers
{
    public class PatientController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHeatDataService _heatData;

        // Map patient login emails to device IDs / CSV prefixes
        // These device IDs must match the CSV filenames in the HeatData folder.
        private static readonly Dictionary<string, string> EmailToDevice = new()
        {
            ["amelia@test.com"] = "d13043b3",
            ["bilal@test.com"] = "de0e9b2c",
            ["chloe@test.com"] = "1c0fd777",
            ["daniel@test.com"] = "71e66ab3",
            ["emily@test.com"] = "543d4676",
        };

        public PatientController(AppDbContext context, IHeatDataService heatData)
        {
            _context = context;
            _heatData = heatData;
        }

        private async Task<(User user, Patient patient)?> GetCurrentUserAndPatientAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return null;

            var user = await _context.Users
                .Include(u => u.PatientProfile)
                .FirstOrDefaultAsync(u => u.UserID == userId.Value);

            if (user == null || user.PatientProfile == null)
                return null;

            return (user, user.PatientProfile);
        }

        // GET: /Patient/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var up = await GetCurrentUserAndPatientAsync();
            if (up == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var (user, patient) = up.Value;

            // Map email to device ID (used to find CSV files)
            if (!EmailToDevice.TryGetValue(user.Email.ToLowerInvariant(), out var deviceId))
            {
                var emptyVm = new PatientDashboardViewModel
                {
                    PatientName = user.FullName ?? user.Email,
                    Email = user.Email
                };
                ViewBag.Error = "No device is linked to your account.";
                return View(emptyVm);
            }

            // Load CSV heatmap + trend
            var heatmapResult = _heatData.GetLatestHeatmap(deviceId);
            var trend = _heatData.GetPeakTrend(deviceId) ?? new List<TrendPoint>();

            double peakPressure = 0;
            double contactArea = 0;

            if (heatmapResult != null && heatmapResult.Values != null)
            {
                var rows = heatmapResult.Values.GetLength(0);
                var cols = heatmapResult.Values.GetLength(1);
                int totalCells = rows * cols;
                int activeCells = 0;

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        var v = heatmapResult.Values[r, c];
                        if (v > peakPressure) peakPressure = v;
                        if (v > 0) activeCells++;
                    }
                }

                if (totalCells > 0)
                {
                    contactArea = (double)activeCells / totalCells * 100.0;
                }
            }

            var vm = new PatientDashboardViewModel
            {
                PatientName = user.FullName ?? user.Email,
                Email = user.Email,
                PeakPressure = peakPressure,
                ContactArea = Math.Round(contactArea, 1),
                LastUpdated = heatmapResult?.Timestamp,
                Heatmap = heatmapResult?.Values,
                PeakTrend = trend
            };

            return View(vm);
        }

        // POST: /Patient/AddDashboardComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddDashboardComment(string content)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["CommentError"] = "Please enter a note before submitting.";
            }
            else
            {
                TempData["CommentSuccess"] = "Your note has been recorded for this session.";
                TempData["LatestNote"] = content.Trim();
            }

            return RedirectToAction("Dashboard");
        }
    }
}
