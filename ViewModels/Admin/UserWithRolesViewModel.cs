namespace Job_Portal_Project.ViewModels.Admin
{
    public class UserWithRolesViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Title { get; set; }
        public string CurrentRole { get; set; }
        public List<string> AllRoles { get; set; }
    }
}
