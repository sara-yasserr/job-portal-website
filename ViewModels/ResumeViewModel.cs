// ResumeViewModel.cs
using System;
using System.Collections.Generic;

namespace Job_Portal_Project.ViewModels
{
    public class ResumeViewModel
    {
        public string ProfessionalSummary { get; set; }
        public IEnumerable<string> Skills { get; set; } = new List<string>();
        public List<WorkExperienceViewModel> WorkExperiences { get; set; } = new();
        public List<EducationViewModel> Educations { get; set; } = new();
    }

    public class WorkExperienceViewModel
    {
        public string Title { get; set; }
        public string Company { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class EducationViewModel
    {
        public string Degree { get; set; }
        public string Institution { get; set; }
        public string FieldOfStudy { get; set; }
        public DateTime EndDate { get; set; }
    }
}