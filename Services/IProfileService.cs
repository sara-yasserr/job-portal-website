using Job_Portal_Project.Models;
using Microsoft.AspNetCore.Http;

namespace Job_Portal_Project.Services
{
    public interface IProfileService
    {
        Task<bool> UpdateProfileAsync(ApplicationUser user);
        Task<string> UploadProfilePictureAsync(string userId, IFormFile profilePicture);
    
        Task<string> UploadResumeAsync(string userId, IFormFile resume);
        Task<List<string>> GetUserResumesAsync(string userId);
        Task<bool> DeleteResumeAsync(string userId, string resumePath);
      
    }
}