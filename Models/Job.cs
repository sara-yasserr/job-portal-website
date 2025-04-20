using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Job_Portal_Project.Models
{
    public class Job
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string EmploymentType { get; set; } //Full-time, Part-time, Internship, etc.
        public string WorkMode { get; set; } //Remote, On-site, Hybrid
        public string City { get; set; }
        public string Country { get; set; }
        public string ExperienceLevel { get; set; }
        public DateTime DatePosted { get; set; }
        public DateTime? DateClosed { get; set; }
        public int VacancyCount { get; set; }
        public bool IsActive { get; set; } = true;

        //Navigation Properties

        public int JobCategoryId { get; set; }
        [ForeignKey("JobCategoryId")]
        public virtual JobCategory JobCategory { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        public string EmployerId { get; set; }
        [ForeignKey("EmployerId")]
        public virtual ApplicationUser Employer { get; set; }
        public virtual List<JobApplication> Applications { get; set; } //List of Applications
    }
}
