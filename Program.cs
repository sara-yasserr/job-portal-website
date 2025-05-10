using Job_Portal_Project.Controllers.Profile;
//using Job_Portal_Project.Data;
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
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;

using System.Security.Claims;
using Job_Portal_Project.ViewModels;
namespace Job_Portal_Project
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            
            var builder = WebApplication.CreateBuilder(args);

            //Add services to the container.

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.Scope.Add("email");
    options.Scope.Add("profile");
    options.SaveTokens = true;

    options.Events = new OAuthEvents
    {
        OnRedirectToAuthorizationEndpoint = context =>
        {

            context.Properties.SetString("returnUrl", context.Request.Query["returnUrl"]);
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        },
        OnCreatingTicket = async ctx =>
        {
            var email = ctx.Identity.FindFirst(ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(email))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.NameIdentifier, ctx.Identity.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                };

                var identity = new ClaimsIdentity(claims, "Google");
                ctx.Principal.AddIdentity(identity);
            }
        }
    };
});

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            // service used insted of  method of on configuration to allow injecting the dbcontext in the repositories without using the service provider
            builder.Services.AddDbContext<JobPortalContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddControllersWithViews();
          
            builder.Services.AddRazorPages();
            builder.Services.AddSession();
            builder.Services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 443;
            });
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
            builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
            builder.Services.AddScoped<IUserMappingService, UserMappingService>();
            builder.Services.AddScoped<IJobCategoryRepository, JobCategoryRepository>();
            builder.Services.AddScoped<IJobApplicationService,JobApplicationService>();
            builder.Services.AddScoped<IJobSearchService, JobSearchService>();
            builder.Services.AddScoped<IJobService, JobService>();
            builder.Services.AddScoped<IFavouritesRepository, FavouritesRepository>();
            builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();
            builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
            builder.Services.AddTransient<IEmailService, EmailService>();

            
            // Services
        
            builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            //builder.Services.AddScoped<JobApplicationService>();
            builder.Services.AddScoped<IJobSearchService, JobSearchService>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBookmarkService, BookmarkService>();

            //builder.Services.AddScoped<IResumeService, ResumeService>();
            //builder.Services.AddScoped<IResumeRepository, ResumeRepository>();
            //builder.Services.AddScoped<JobApplicationService, JobApplicationService>();

            builder.Services.AddScoped<ResumeController>();
     
            var app = builder.Build();



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                //app.UseExceptionHandler("/Error/GlobalError");
                //app.UseStatusCodePagesWithReExecute("/Error/GlobalError", "?statusCode={0}");
                //app.UseHsts();
            }
            else
            {
                //app.UseExceptionHandler("/Error/GlobalError");
                //app.UseStatusCodePagesWithReExecute("/Error/GlobalError", "?statusCode={0}");
                //app.UseHsts();
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
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