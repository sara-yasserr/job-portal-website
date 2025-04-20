using System.Globalization;
using Microsoft.AspNetCore.Identity;

namespace Job_Portal_Project.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Title { get; set; } //If User Is Employer
        public string City { get; set; }
        public string Country { get; set; }
        public string Role { get; set; }
        public string? ProfilePicturePath { get; set; }
        public string? ResumePath { get; set; } //If User Is Applicant
        //Navigation Properties
        public virtual List<Job>? PostedJobs { get; set; } //If User Is Employer
        public virtual List<JobApplication>? JobApplications { get; set; } //If User Is Applicant
        public virtual List<Favourites> Favourites { get; set; } //If User Is Applicant
    }
}
