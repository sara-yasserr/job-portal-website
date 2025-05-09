using Job_Portal_Project.Models;

namespace Job_Portal_Project.ViewModels
{
    public class BookmarkedJobsViewModel
    {
        public Job Job { get; set; }
        public int jobid { get; set; }
        public bool IsBookmarked { get; set; }
        public List<Favourites> BookmarkedJobs { get; set; }
       
    }
}