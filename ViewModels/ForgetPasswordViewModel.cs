using System.ComponentModel.DataAnnotations;

namespace Job_Portal_Project.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [Required, EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
