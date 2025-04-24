using Job_Portal_Project.Models;
using Microsoft.AspNetCore.Identity;

public interface IUserService
{

    Task<ApplicationUser> GetUserByIdAsync(string userId);
    Task<ApplicationUser> GetUserByEmailAsync(string email);
    Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
    Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<List<ApplicationUser>> GetAllUsersAsync();

}