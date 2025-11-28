using System;
using System.Linq;
using GrapheneTrace.Models;
using Microsoft.Extensions.DependencyInjection;

namespace GrapheneTrace.Data
{
    /// <summary>
    /// Seeds 5 demo patient accounts into the Users and Patients tables.
    /// This is idempotent: it will create any missing users or patients,
    /// but will not duplicate existing ones.
    /// </summary>
    public static class DemoSeeder
    {
        public static void Seed(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<AppDbContext>();

            // Random-ish first names, all using @test.com
            var demoPatients = new[]
            {
                new { Email = "amelia@test.com", FullName = "Amelia" },
                new { Email = "bilal@test.com",  FullName = "Bilal"  },
                new { Email = "chloe@test.com",  FullName = "Chloe"  },
                new { Email = "daniel@test.com", FullName = "Daniel" },
                new { Email = "emily@test.com",  FullName = "Emily"  },
            };

            // Default values for required / important fields
            var defaultDob = new DateTime(1990, 1, 1);
            const string defaultEmergencyContactName = "Demo Contact";
            const string defaultEmergencyContactNumber = "0000000000";
            const string defaultMedicalNotes = "Demo patient seeded for testing.";
            const string defaultPassword = "Pass123!";   // demo password

            foreach (var p in demoPatients)
            {
                // -------------------------------------------------
                // 1) Ensure User exists
                // -------------------------------------------------
                var user = context.Users.SingleOrDefault(u => u.Email == p.Email);

                if (user == null)
                {
                    user = new User
                    {
                        Email = p.Email,
                        PasswordHash = defaultPassword,
                        FullName = p.FullName,
                        Role = "Patient",
                        AccountStatus = "Active",
                        CreatedAt = DateTime.Now
                    };

                    context.Users.Add(user);
                    context.SaveChanges(); // generates UserID
                }

                // -------------------------------------------------
                // 2) Ensure Patient exists for this user
                // -------------------------------------------------
                var patient = context.Patients.SingleOrDefault(pt => pt.UserID == user.UserID);

                if (patient == null)
                {
                    patient = new Patient
                    {
                        UserID = user.UserID,
                        DateOfBirth = defaultDob,
                        EmergencyContactName = defaultEmergencyContactName,
                        EmergencyContactNumber = defaultEmergencyContactNumber,
                        MedicalNotes = defaultMedicalNotes
                    };

                    context.Patients.Add(patient);
                    context.SaveChanges();
                }
            }
        }
    }
}
