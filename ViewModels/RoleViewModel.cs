using System.ComponentModel.DataAnnotations;

namespace Job_Portal_Project.ViewModels
{
    public class RoleViewModel
    {
        [Required]
        public string RoleName { get; set; }

    }
}
