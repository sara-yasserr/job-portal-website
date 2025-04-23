using Job_Portal_Project.Models;

public interface IBookmarkService
{
    Task<List<Favourites>> GetBookmarkedJobsAsync(string userId);
    Task<bool> BookmarkJobAsync(string userId, int jobId);
    Task<bool> RemoveBookmarkAsync(string userId, int jobId);
    Task<bool> IsJobBookmarkedAsync(string userId, int jobId);
}