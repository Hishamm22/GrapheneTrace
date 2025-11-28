using System.ComponentModel.DataAnnotations;

namespace GrapheneTrace.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(80)]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password should be at least 6 characters.")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Emergency contact name")]
        public string? EmergencyContactName { get; set; }

        [Display(Name = "Emergency contact number")]
        public string? EmergencyContactNumber { get; set; }
    }
}
