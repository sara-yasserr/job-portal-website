using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Job_Portal_Project.ViewModels.Admin
{
    public class RoleViewModel
    {
        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        public List<IdentityRole>? ExistingRoles { get; set; }

    }
}
