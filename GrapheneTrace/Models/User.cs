using System.ComponentModel.DataAnnotations;

namespace GrapheneTrace.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required, EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        // For the assignment we keep it simple (plain text),
        // but in the real world this should be a secure hash.
        [Required]
        [StringLength(200)]
        public string PasswordHash { get; set; }

        [StringLength(100)]
        public string? FullName { get; set; }

        // "Patient", "Clinician", "Admin", etc.
        [StringLength(50)]
        public string? Role { get; set; }

        [StringLength(50)]
        public string? AccountStatus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public Patient? PatientProfile { get; set; }
    }
}
