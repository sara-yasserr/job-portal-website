using Job_Portal_Project.Controllers.Profile;
using Job_Portal_Project.Models;
using Job_Portal_Project.Models.DbContext;
using Job_Portal_Project.Repositories;
using Job_Portal_Project.Repositories.ApplicationUserRepository;
using Job_Portal_Project.Services;
using Job_Portal_Project.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_Project
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(options =>
            //    {
            //        options.LoginPath = "/Account/Login";
            //    })
            //    .AddGoogle(options =>
            //    {
            //        options.ClientId = "";
            //        options.ClientSecret = "";
            //    });

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            // service used insted of  method of on configuration to allow injecting the dbcontext in the repositories without using the service provider
            builder.Services.AddDbContext<JobPortalContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddControllersWithViews();
          
            builder.Services.AddRazorPages();
            builder.Services.AddSession();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireUppercase = false;
            }
            ).AddEntityFrameworkStores<JobPortalContext>().AddDefaultTokenProviders();

            //Repositories  servives 
            builder.Services.AddScoped<IUserMappingService, UserMappingService>();
            builder.Services.AddScoped<IJobRepository, JobRepository>();
            builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
            builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
            builder.Services.AddScoped<IUserMappingService, UserMappingService>();
            builder.Services.AddScoped<IJobCategoryRepository, JobCategoryRepository>();

            
            // Services
        
            builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            builder.Services.AddScoped<IJobSearchService, JobSearchService>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBookmarkService, BookmarkService>();

            //builder.Services.AddScoped<IResumeService, ResumeService>();
            //builder.Services.AddScoped<IResumeRepository, ResumeRepository>();

            builder.Services.AddScoped<ResumeController>();
           





            var app = builder.Build();



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();
            app.UseStaticFiles();


            app.Run();
        }
    }
}