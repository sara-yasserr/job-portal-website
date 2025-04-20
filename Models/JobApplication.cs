using System.ComponentModel.DataAnnotations;

namespace Job_Portal_Project.Models
{
    public class JobApplication
    {
        [Key]
        public int Id { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; } = "Pending";
        public string? SpecificResumePath { get; set; } //Path to the resume file (Specific to the job)

        //Navigation Properties
        public int JobId { get; set; }
        public virtual Job Job { get; set; }

        public string ApplicantId { get; set; }
        public virtual ApplicationUser Applicant { get; set; }
    }
}
