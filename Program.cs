using Job_Portal_Project.Controllers.Profile;
//using Job_Portal_Project.Data;
using System.Security.Claims;
using Job_Portal_Project.Models;
using Job_Portal_Project.Models.DbContext;
using Job_Portal_Project.Repositories;
using Job_Portal_Project.Repositories.ApplicationUserRepository;
using Job_Portal_Project.Services;
using Job_Portal_Project.Services.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_Project
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            
            var builder = WebApplication.CreateBuilder(args);

            //Add services to the container.

<<<<<<< HEAD
           //builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
           //    .AddCookie(options =>
           //    {
           //        options.LoginPath = "/Account/Login";
           //    })
           //    .AddGoogle(options =>
           //    {
           //        options.ClientId = "";
           //        options.ClientSecret = "";
           //        options.Scope.Add("email");
           //        options.Scope.Add("profile");
           //        options.SaveTokens = true;

           //        options.Events.OnCreatingTicket = ctx =>
           //        {
           //            var email = ctx.User.GetProperty("email").GetString();

           //            if (!string.IsNullOrEmpty(email))
           //            {
           //                var claims = new List<Claim>
           //                {
           //                   new Claim(ClaimTypes.Email, email)
           //                };

           //                var identity = new ClaimsIdentity(claims, "Google");
           //                var principal = new ClaimsPrincipal(identity);
           //                ctx.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
           //            }

           //            return Task.CompletedTask;
           //        };
           //    });
=======
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                })
                .AddGoogle(options =>
                {
                    options.ClientId = "Authentication:Google:ClientId";
                    options.ClientSecret = "Authentication:Google: ClientSecret";
                    options.Scope.Add("email");
                    options.Scope.Add("profile");
                    options.SaveTokens = true;

                    options.Events.OnCreatingTicket = ctx =>
                    {
                        var email = ctx.User.GetProperty("email").GetString();

                        if (!string.IsNullOrEmpty(email))
                        {
                            var claims = new List<Claim>
                            {
                              new Claim(ClaimTypes.Email, email)
                            };

                            var identity = new ClaimsIdentity(claims, "Google");
                            var principal = new ClaimsPrincipal(identity);
                            ctx.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        }

                        return Task.CompletedTask;
                    };
                });
>>>>>>> e5124330c55b98497ef6d0c2e96df8a5d744b568

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
            builder.Services.AddScoped<IJobService, JobService>();
            builder.Services.AddScoped<IFavouritesRepository, FavouritesRepository>();
            builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();


            
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

            app.UseStaticFiles();


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