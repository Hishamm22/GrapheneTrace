using GrapheneTrace.Data;
using GrapheneTrace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GrapheneTrace.Controllers
{
    public class PatientController : Controller
    {
        private readonly AppDbContext _context;

        public PatientController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Patient/Dashboard
        // Shows a simple overview and a list of recent sessions.
        public async Task<IActionResult> Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("UserRole");

            if (userId == null || role != "Patient")
            {
                // If not logged in as patient, send back to login.
                return RedirectToAction("Login", "Account");
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserID == userId.Value);

            if (patient == null)
            {
                ViewBag.Error = "Patient profile not found.";
                ViewBag.PatientName = HttpContext.Session.GetString("UserName") ?? "Patient";
                ViewBag.Sessions = new List<SensorSession>();
                return View();
            }

            var sessions = await _context.SensorSessions
                .Where(s => s.PatientID == patient.PatientID)
                .OrderByDescending(s => s.StartTime)
                .Take(5)
                .ToListAsync();

            ViewBag.PatientName = HttpContext.Session.GetString("UserName") ?? "Patient";
            ViewBag.PatientId = patient.PatientID;
            ViewBag.Sessions = sessions;

            return View();
        }

        // GET: /Patient/ViewSession/5?frameIndex=0
        // Shows details for a specific session, including a "current frame"
        // and the comments attached to that session/frame.
        public async Task<IActionResult> ViewSession(int sessionId, int frameIndex = 0)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("UserRole");

            if (userId == null || role != "Patient")
            {
                return RedirectToAction("Login", "Account");
            }

            // Find the patient for this user.
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserID == userId.Value);

            if (patient == null)
            {
                return RedirectToAction("Dashboard");
            }

            // Make sure the session belongs to this patient.
            var session = await _context.SensorSessions
                .FirstOrDefaultAsync(s => s.SessionID == sessionId && s.PatientID == patient.PatientID);

            if (session == null)
            {
                // Either invalid session ID or not owned by this patient.
                return RedirectToAction("Dashboard");
            }

            // Load frames for this session, ordered by FrameIndex.
            var frames = await _context.SensorFrames
                .Where(f => f.SessionID == sessionId)
                .OrderBy(f => f.FrameIndex)
                .ToListAsync();

            if (!frames.Any())
            {
                ViewBag.Error = "No frames recorded for this session.";
                ViewBag.Session = session;
                ViewBag.CurrentFrame = null;
                ViewBag.CurrentFrameIndex = 0;
                ViewBag.TotalFrames = 0;
                ViewBag.Comments = new List<Comment>();
                return View();
            }

            // Clamp frameIndex so it is always in range.
            if (frameIndex < 0) frameIndex = 0;
            if (frameIndex >= frames.Count) frameIndex = frames.Count - 1;

            var currentFrame = frames[frameIndex];

            // Load comments for this session (both general and frame-specific).
            var comments = await _context.Comments
                .Where(c => c.SessionID == sessionId &&
                       (c.FrameID == null || c.FrameID == currentFrame.FrameID))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            ViewBag.PatientName = HttpContext.Session.GetString("UserName") ?? "Patient";
            ViewBag.PatientId = patient.PatientID;

            ViewBag.Session = session;
            ViewBag.CurrentFrame = currentFrame;
            ViewBag.CurrentFrameIndex = frameIndex;
            ViewBag.TotalFrames = frames.Count;
            ViewBag.Comments = comments;

            return View();
        }

        // POST: /Patient/AddComment
        // Saves a new note from the patient for a given session/frame.
        [HttpPost]
        public async Task<IActionResult> AddComment(int sessionId, int? frameId, int currentFrameIndex, string content)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("UserRole");

            if (userId == null || role != "Patient")
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                // Don't save empty comments; just come back to the session view.
                TempData["CommentError"] = "Please enter a note before saving.";
                return RedirectToAction("ViewSession", new { sessionId = sessionId, frameIndex = currentFrameIndex });
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserID == userId.Value);

            if (patient == null)
            {
                return RedirectToAction("Dashboard");
            }

            var comment = new Comment
            {
                PatientID = patient.PatientID,
                SessionID = sessionId,
                FrameID = frameId,
                AuthorUserID = userId.Value,
                CreatedAt = DateTime.Now,
                Content = content,
                IsClinicianReply = false
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            TempData["CommentSuccess"] = "Your note has been saved.";

            return RedirectToAction("ViewSession", new { sessionId = sessionId, frameIndex = currentFrameIndex });
        }
    }
}
