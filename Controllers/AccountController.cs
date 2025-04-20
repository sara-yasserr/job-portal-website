using System.Security.Claims;
using Job_Portal_Project.Models;
using Job_Portal_Project.Repositories.ApplicationUserRepository;
using Job_Portal_Project.Services;
using Job_Portal_Project.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_Project.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;
        IApplicationUserRepository _applicationUserRepository;
        IUserMappingService _userMappingService;
        public AccountController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager ,
            IApplicationUserRepository applicationUserRepository , IUserMappingService userMappingService)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            _applicationUserRepository = applicationUserRepository;
            _userMappingService = userMappingService;
        }

        #region Register
        public IActionResult Register()
        {
            return View();  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel userVM)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _applicationUserRepository.GetByUserNameAsync(userVM.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Username already exists");
                    return View("Register", userVM);
                }
                ApplicationUser userFromDB = _userMappingService.MapToApplicationUser(userVM);
                //create db
                IdentityResult result = await userManager.CreateAsync(userFromDB, userVM.Password);

                //create cookie
                if (result.Succeeded)
                {
                    //to assign admins ==>>
                    //await userManager.AddToRoleAsync(userFromDB, "Admin");
                    string selectRole = userVM.Role.ToString();
                    await userManager.AddToRoleAsync(userFromDB, selectRole);
                    await signInManager.SignInAsync(userFromDB,false);
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View("Register", userVM);
        }

        #endregion

        #region Login
  
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {    //check
                ApplicationUser userFromDB = await userManager.FindByNameAsync(loginVM.UserName);

                if (userFromDB != null)
                {

                    bool found = await userManager.CheckPasswordAsync(userFromDB, loginVM.Password);
                    if (found)
                    {
                        //create cookie
                        List<Claim> claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, userFromDB.Id));
                        claims.Add(new Claim(ClaimTypes.Name, userFromDB.UserName));
                        claims.Add(new Claim(ClaimTypes.Email, userFromDB.Email));
                        claims.Add(new Claim("FirstName", userFromDB.FirstName));
                        claims.Add(new Claim("LastName", userFromDB.LastName));
                        claims.Add(new Claim("City", userFromDB.City));
                        claims.Add(new Claim("Country", userFromDB.Country));
                        claims.Add(new Claim(ClaimTypes.Role, userFromDB.Role));

                        //await signInManager.SignInAsync(userFromDB, loginVM.RememberMe);
                        await signInManager.SignInWithClaimsAsync(userFromDB, loginVM.RememberMe,claims);

                        return RedirectToAction("Index", "Home");
                    }   
                }

                ModelState.AddModelError("", "Username or Password is invalid");

            }
            return View("Login", loginVM);
        }
        #endregion
        #region External login 
        public IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult GoogleLogin(string returnUrl = null)
        {
            var redirectUrl = Url.Action("GoogleCallBack", "Account", new { returnUrl });
            var properries = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properries, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleCallBack(string returnUrl = null)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToLocal(returnUrl);
        }
        #endregion

        #region Logout
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        #endregion
    }
}
