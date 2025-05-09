using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace Job_Portal_Project.ViewModels
{
    public class ProfileViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        public string Role { get; set; }


        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }

        public string ProfilePicturePath { get; set; }

        [Display(Name = "Resume")]
        public IFormFile Resume { get; set; }

        public string ResumePath { get; set; }
    }
}