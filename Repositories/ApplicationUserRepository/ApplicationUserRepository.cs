using Job_Portal_Project.Models;
using Job_Portal_Project.Models.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_Project.Repositories.ApplicationUserRepository
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JobPortalContext context;

        public ApplicationUserRepository(UserManager<ApplicationUser> userManager, JobPortalContext context)
        {
            _userManager = userManager;
            this.context = context;
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
            var user = await _userManager.FindByNameAsync(userName);
            return user;
        }
        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new InvalidOperationException($"User with email '{email}' not found.");
            }
            return user;
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

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

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
