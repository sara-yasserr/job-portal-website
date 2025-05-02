using Job_Portal_Project.Models;
using Job_Portal_Project.Repositories;
using Job_Portal_Project.Repositories.ApplicationUserRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Job_Portal_Project.Services
{
    public class ProfileService : IProfileService
    {
        
        private readonly IApplicationUserRepository _userRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileService(IApplicationUserRepository userRepository, IWebHostEnvironment webHostEnvironment)
        {
            _userRepository = userRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> UpdateProfileAsync(ApplicationUser user)
        {
            var result = await _userRepository.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<string> UploadProfilePictureAsync(string userId, IFormFile profilePicture)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || profilePicture == null || profilePicture.Length == 0)
                return user?.ProfilePicturePath;

            // Delete old picture if exists
            if (!string.IsNullOrEmpty(user.ProfilePicturePath))
            {
                var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfilePicturePath.TrimStart('~', '/'));
                if (File.Exists(oldPath))
                {
                    File.Delete(oldPath);
                }
            }

            // Save new picture
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profiles");
            Directory.CreateDirectory(uploadsFolder); // No need to check exists

            string uniqueFileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(profilePicture.FileName)}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await profilePicture.CopyToAsync(fileStream);
            }

            // Store path without ~ or / at start
            string relativePath = $"images/profiles/{uniqueFileName}";
            user.ProfilePicturePath = relativePath;
            await _userRepository.UpdateAsync(user);

            return relativePath;
        }

        public async Task<string> UploadResumeAsync(string userId, IFormFile resume)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || resume == null || resume.Length == 0) return user?.ResumePath;

            // Delete old resume if exists
            if (!string.IsNullOrEmpty(user.ResumePath))
            {
                var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, user.ResumePath.TrimStart('/'));
                if (File.Exists(oldPath))
                {
                    File.Delete(oldPath);
                }
            }

            // Save resume
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "files", "resumes");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = $"{userId}_{Guid.NewGuid().ToString()}_{resume.FileName}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await resume.CopyToAsync(fileStream);
            }

            string relativePath = $"/files/resumes/{uniqueFileName}";
            user.ResumePath = relativePath;
            await _userRepository.UpdateAsync(user);

            return relativePath;
        }

        public Task<List<string>> GetUserResumesAsync(string userId)
        {
            // In the current model, we only have one resume per user
            // This method would be expanded if we implement multiple resumes
            throw new NotImplementedException("Multiple resumes not implemented yet");
        }
        public async Task<bool> DeleteResumeAsync(string userId, string resumePath)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.ResumePath)) return false;

            // Delete the resume file
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ResumePath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Update user record
            user.ResumePath = null;
            var result = await _userRepository.UpdateAsync(user);
            return result.Succeeded;
        }


        


    }
}