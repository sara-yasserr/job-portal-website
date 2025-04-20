using Job_Portal_Project.Models;
using Job_Portal_Project.ViewModels;

namespace Job_Portal_Project.Services
{
    public class UserMappingService : IUserMappingService
    {
        public ApplicationUser MapToApplicationUser(RegisterViewModel vm)
        {
            return new ApplicationUser
            {
                UserName = vm.UserName,
                Email = vm.Email,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                City = vm.City,
                Country = vm.Country,
                Role = vm.Role.ToString(),
                ProfilePicturePath = string.IsNullOrWhiteSpace(vm.ProfilePicturePath)
                    ? "/images/default-profile.png"
                    : vm.ProfilePicturePath
            };
        }
    }
}
