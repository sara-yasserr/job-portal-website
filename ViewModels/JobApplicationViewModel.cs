using System.ComponentModel.DataAnnotations;
using Job_Portal_Project.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Job_Portal_Project.ViewModels
{
    public class JobApplicationViewModel
    {
        public int JobId { get; set; }
        public string ApplicantId { get; set; }
        public string? ResumePath { get; set; }
        public IFormFile? ResumeFile { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; } = "Pending";
        [ValidateNever]
        public virtual Job job { get; set; }
        [ValidateNever]
        public virtual ApplicationUser Applicant { get; set; }
        [Required(ErrorMessage = "You must accept the terms to continue.")]
        public bool Consent { get; set; }
    }
}