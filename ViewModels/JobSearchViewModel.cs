using Job_Portal_Project.Models;
using System.ComponentModel.DataAnnotations;


namespace Job_Portal_Project.ViewModels
{
    public class JobSearchViewModel
    {
        // search  filters 
        public string? Keyword { get; set; } //word to search for in the job title or description
        public int? CategoryId { get; set; }  //id of category to filter jobs by 1>software, 2>marketing, ......
        public string? Location { get; set; } //cairo , alexandria.
        public string? EmploymentType { get; set; } // part-time, full-time, internship, etc.
        public string? WorkMode { get; set; }  // remote, on-site, hybrid
        public string? ExperienceLevel { get; set; }  // entry-level, mid-level, senior-level

        // current user information to display in the view
        public string? CurrentUserId { get; set; }
        public ApplicationUser? CurrentUser { get; set; }

        // filter options lists
        public List<JobCategory> Categories { get; set; } = new();
        public List<string> Locations { get; set; } = new();
        public List<string> EmploymentTypes { get; set; } = new();
        public List<string> WorkModes { get; set; } = new();
        public List<string> ExperienceLevels { get; set; } = new(); //

        //search results after applying filters
        public List<Job> Jobs { get; set; } = new();  //list of jobs that match the search criteria
        public List<Job> RecommendedJobs { get; set; } = new(); //recommended jobs based on the user's profile and search history`


        //catigories
        public string? CategoryName { get; set; }

        public List<Favourites> BookmarkedJobs { get; set; }
        public bool IsBookmarked { get; set; }



        // browser pagination 
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalJobs { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalJobs / (double)PageSize);

    }
}





