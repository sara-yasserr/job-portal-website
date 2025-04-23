using Job_Portal_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_Project.Repositories.ApplicationUserRepository
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ApplicationUserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<ApplicationUser>> GetAllAsync()
        {
            return await _userManager.Users.ToListAsync();
        }
        
        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }
        public async Task<ApplicationUser> GetByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }
        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> InsertAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));

            return await _userManager.DeleteAsync(user);
        }

        public Task<int> GetNumberOfUsersAsync()
        {
            return _userManager.Users.CountAsync();
        }
        public Task<int> GetNumberOfApplicantsAsync()
        {
            return _userManager.Users.Where(u => u.Role == "JobSeeker").CountAsync();
        }
        public Task<int> GetNumberOfEmployersAsync()
        {
            return _userManager.Users.Where(u => u.Role == "Employer").CountAsync();
        }

    }
}
