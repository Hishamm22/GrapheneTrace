using GrapheneTrace.Models;
using Microsoft.EntityFrameworkCore;

namespace GrapheneTrace.Data
{
    // Simple helper to insert demo data so the UI has something to show.
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // Make sure database exists.
            context.Database.EnsureCreated();

            // If there is already a user, assume we have data and skip seeding.
            if (context.Users.Any())
                return;

            // 1) Create a demo patient user.
            var user = new User
            {
                Email = "patient@example.com",
                // For the assignment we keep it simple: store plain password.
                PasswordHash = "Password123!",
                FullName = "Demo Patient",
                Role = "Patient",
                AccountStatus = "Active",
                CreatedAt = DateTime.Now
            };

            context.Users.Add(user);
            context.SaveChanges(); // so UserID is generated

            // 2) Create the patient profile linked to this user.
            var patient = new Patient
            {
                UserID = user.UserID,
                DateOfBirth = new DateTime(1990, 1, 1),
                EmergencyContactName = "Jane Doe",
                EmergencyContactNumber = "07123456789",
                MedicalNotes = "Example patient used for demonstration."
            };

            context.Patients.Add(patient);
            context.SaveChanges(); // PatientID generated

            // 3) Create a sensor session for this patient.
            var session = new SensorSession
            {
                PatientID = patient.PatientID,
                StartTime = DateTime.Now.AddMinutes(-30),
                EndTime = DateTime.Now,
                RecordingArea = "Wheelchair seat",
                SourceDeviceID = "DEVICE-001",
                DataFilePath = "demo.csv",
                AlgorithmDescription = "Demo algorithm for assignment."
            };

            context.SensorSessions.Add(session);
            context.SaveChanges(); // SessionID generated

            // 4) Create a few frames for this session.
            var frames = new List<SensorFrame>
            {
                new SensorFrame
                {
                    SessionID = session.SessionID,
                    FrameIndex = 0,
                    Timestamp = session.StartTime.AddMinutes(5),
                    PeakPressureIndex = 120,
                    AveragePressure = 60,
                    ContactAreaPercent = 45,
                    ThermalReactivityScore = 0.6f,
                    MinRange = 0,
                    MaxRange = 255
                },
                new SensorFrame
                {
                    SessionID = session.SessionID,
                    FrameIndex = 1,
                    Timestamp = session.StartTime.AddMinutes(10),
                    PeakPressureIndex = 150,
                    AveragePressure = 70,
                    ContactAreaPercent = 50,
                    ThermalReactivityScore = 0.8f,
                    MinRange = 0,
                    MaxRange = 255
                },
                new SensorFrame
                {
                    SessionID = session.SessionID,
                    FrameIndex = 2,
                    Timestamp = session.StartTime.AddMinutes(15),
                    PeakPressureIndex = 180,
                    AveragePressure = 75,
                    ContactAreaPercent = 55,
                    ThermalReactivityScore = 0.9f,
                    MinRange = 0,
                    MaxRange = 255
                }
            };

            context.SensorFrames.AddRange(frames);
            context.SaveChanges();

            // 5) Add an example comment from the patient on the second frame.
            var comment = new Comment
            {
                PatientID = patient.PatientID,
                SessionID = session.SessionID,
                FrameID = frames[1].FrameID,
                AuthorUserID = user.UserID,
                CreatedAt = DateTime.Now,
                Content = "Felt some discomfort around this time.",
                IsClinicianReply = false
            };

            context.Comments.Add(comment);
            context.SaveChanges();
        }
    }
}
