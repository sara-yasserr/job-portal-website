using System.ComponentModel.DataAnnotations;
using Job_Portal_Project.Attributes;
using Job_Portal_Project.Repositories.ApplicationUserRepository;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_Project.ViewModels
{
    public enum Role
    {
        JobSeeker,
        Employer,
        Admin
    }
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please enter a username.")]
        [Remote(action: "IsUniqueUserName", controller: "Account", ErrorMessage = "Username is already taken")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
        [Display(Name = "Username")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Password must include an uppercase letter, a lowercase letter, a number, and a special character.")]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Remote(action: "IsUniqueEmail", controller: "Account", ErrorMessage = "Email is already taken")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(50, ErrorMessage = "Title cannot be longer than 50 characters.")]
        [Display(Name = "Title (Optional)")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Please enter your first name.")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name.")]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Please enter your city.")]
        [StringLength(50, ErrorMessage = "Country name cannot be longer than 50 characters.")]
        [Display(Name = "City")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Please enter your country.")]
        [StringLength(50, ErrorMessage = "Country name cannot be longer than 50 characters.")]
        [Display(Name = "Country")]
        public string? Country { get; set; }

        [Required(ErrorMessage = "Please select an account type.")]
        [Display(Name = "Account Type")]
        public Role? Role { get; set; } // Employer or Applicant

        public string? ProfilePicturePath { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Profile Picture (Optional)")]
        [FileExtensions(Extensions = "jpg,jpeg,png", ErrorMessage = "Only JPG, JPEG, or PNG files are allowed.")]
        public IFormFile? ProfilePictureFile { get; set; }
    }

}
