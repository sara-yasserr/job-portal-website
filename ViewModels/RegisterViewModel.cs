using System.ComponentModel.DataAnnotations;

namespace Job_Portal_Project.ViewModels
{
    public enum Role
    {
        JobSeeker,
        Employer
    }
    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string? Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public Role? Role { get; set; } // Employer or Applicant
        public string? ProfilePicturePath { get; set; }
        public IFormFile? ProfilePictureFile { get; set; }

    }
}
