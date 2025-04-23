using Job_Portal_Project.Models;
using Microsoft.AspNetCore.Identity;

namespace Job_Portal_Project.Repositories.ApplicationUserRepository
{
    public interface IApplicationUserRepository
    {
        // Basic User Operations
        Task<List<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<ApplicationUser> GetByUserNameAsync(string userName);
        Task<ApplicationUser> GetByEmailAsync(string email);
        Task<IdentityResult> InsertAsync(ApplicationUser user, string password);
        Task<IdentityResult> UpdateAsync(ApplicationUser user);
        Task<IdentityResult> DeleteAsync(ApplicationUser user);

                                  
    }
}

