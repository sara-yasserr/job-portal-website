using Job_Portal_Project.Models;
using Job_Portal_Project.ViewModels;

namespace Job_Portal_Project.Services
{
    public interface IUserMappingService
    {
        ApplicationUser MapToApplicationUser(RegisterViewModel registerViewModel);
        public void MapToUpdateUser(ApplicationUser existingUser, ApplicationUser updatedData);
    }
}