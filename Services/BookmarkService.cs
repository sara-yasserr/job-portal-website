// Services/BookmarkService.cs
using Job_Portal_Project.Models;
using Job_Portal_Project.Models.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_Project.Services
{
    public class BookmarkService : IBookmarkService
    {
        private readonly JobPortalContext _context;

        public BookmarkService(JobPortalContext context)
        {
            _context = context;
        }

        public async Task<List<Favourites>> GetBookmarkedJobsAsync(string userId)
        {
            return await _context.Favourites
                .Include(b => b.Job)
                .ThenInclude(j => j.Company)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> BookmarkJobAsync(string userId, int jobId)
        {
            if (await IsJobBookmarkedAsync(userId, jobId))
                return false;

            var bookmark = new Favourites
            {
                JobId = jobId,
                UserId = userId,
                DateAdded = DateTime.Now
            };

            _context.Favourites.Add(bookmark);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveBookmarkAsync(string userId, int jobId)
        {
            var bookmark = await _context.Favourites
                .FirstOrDefaultAsync(b => b.UserId == userId && b.JobId == jobId);

            if (bookmark == null)
                return false;

            _context.Favourites.Remove(bookmark);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsJobBookmarkedAsync(string userId, int jobId)
        {
            return await _context.Favourites
                .AnyAsync(b => b.UserId == userId && b.JobId == jobId);
        }
    }
}
