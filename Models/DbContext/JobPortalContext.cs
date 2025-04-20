using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_Project.Models.DbContext
{
    public class JobPortalContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<JobApplication> JobApplications { get; set; }
        public virtual DbSet<Favourites> Favourites { get; set; }
        public virtual DbSet<JobCategory> JobCategories { get; set; }

        public JobPortalContext(DbContextOptions<JobPortalContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            string adminRoleId = "ADMIN-ROLE-001";
            string EmployeerRoleId = "Employer-ROLE-001";
            string ApplicantRoleId = "JobSeeker-ROLE-001";

            string adminUserId = "ADMIN-USER-001";

            // Admin Role
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            { 
                Id = EmployeerRoleId,
                Name = "Employer",
                NormalizedName = "EMPLOYER"
            },
            new IdentityRole
            {
                Id = ApplicantRoleId,
                Name = "JobSeeker",
                NormalizedName = "JOBSEEKER"
            });

            // Admin User
            builder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@site.com",
                NormalizedUserName = "ADMIN@SITE.COM",
                Email = "admin@jobportal.com",
                NormalizedEmail = "ADMIN@JOBPORTAL.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEIjJh6/LXD2Bg+3MJGc+CmiaE471FJWBEmlTQ/1OhqkFw0NIgG/beU7wkTfmnuQ/sQ==",
                SecurityStamp = "STATIC-SECURITY-STAMP-001",
                ConcurrencyStamp = "STATIC-CONCURRENCY-STAMP-001",
                FirstName = "Admin",
                LastName = "User",
                Title = "Administrator",
                Role = "Admin",
                City = "Cairo",
                Country = "Egypt",
                ProfilePicturePath = "/images/default-profile.png"
            });

            // Assign Admin Role
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                UserId = adminUserId,
                RoleId = adminRoleId
            });
        }
    }
}
