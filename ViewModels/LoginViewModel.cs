using System.ComponentModel.DataAnnotations;

namespace Job_Portal_Project.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Username")]
        public string? UserName { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
