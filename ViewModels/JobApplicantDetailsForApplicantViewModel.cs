namespace Job_Portal_Project.ViewModels
{
    public class JobApplicantDetailsForApplicantViewModel
    {
        public string ApplicantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string SpecificResumePath { get; set; }
        public string ProfilePicturePath { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string status { get; set; }
        public int JobId { get; set; }
    }
}
