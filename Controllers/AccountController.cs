using System.Security.Claims;
using Job_Portal_Project.Models;
using Job_Portal_Project.Repositories.ApplicationUserRepository;
using Job_Portal_Project.Services;
using Job_Portal_Project.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
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
        private readonly IUserService _userService;
        public AccountController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager ,
            IApplicationUserRepository applicationUserRepository, IUserService userService, IUserMappingService userMappingService)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            _applicationUserRepository = applicationUserRepository;
            _userMappingService = userMappingService;
            _userService = userService;
        }

        #region Register
        //Identifying Role = Employer | JobSeeker
        public IActionResult PreRegister()
        {
            return View("PreRegister");
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult PreRegister(RegisterViewModel userVM)
        {
            if (userVM.Role != null)
            {
                return RedirectToAction("Register", userVM);
            }
            return View("PreRegister", userVM);
        }
        //Redirecting to Register with specified role
        public IActionResult Register(Role Role, string Email = null)
        {
            RegisterViewModel userVM = new RegisterViewModel();
            userVM.Role = Role;
            if (!string.IsNullOrEmpty(Email)) { userVM.Email = Email; }
            return View("Register", userVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel userVM)
        {
            if (userVM.ProfilePictureFile != null && userVM.ProfilePictureFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + userVM.ProfilePictureFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    userVM.ProfilePictureFile.CopyTo(fileStream);
                }

                userVM.ProfilePicturePath = uniqueFileName;
            }
            //to assign admins ==>>
            // userVM.Role = Role.Admin;
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
                    string selectRole = userVM.Role.ToString();
                    await userManager.AddToRoleAsync(userFromDB, selectRole);
                    await signInManager.SignInAsync(userFromDB, false);
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                    return View("Register", userVM);
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
                        claims.Add(new Claim(ClaimTypes.Email, userFromDB.Email ?? string.Empty));
                        claims.Add(new Claim("FirstName", userFromDB.FirstName ));
                        claims.Add(new Claim("LastName", userFromDB.LastName));
                        claims.Add(new Claim("City", userFromDB.City));
                        claims.Add(new Claim("Country", userFromDB.Country));
                        claims.Add(new Claim(ClaimTypes.Role, userFromDB.Role));

                        //await signInManager.SignInAsync(userFromDB, loginVM.RememberMe);
                        await signInManager.SignInWithClaimsAsync(userFromDB, loginVM.RememberMe, claims);

                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("", "Username or Password is invalid");

            }
            return View("Login", loginVM);
        }
        #endregion
        #region External login 
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) ||
                returnUrl.Equals("/Account/Login", StringComparison.OrdinalIgnoreCase) ||
                returnUrl.StartsWith("/Identity/Account/Login", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Home");
            }

            if (Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult GoogleLogin(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleCallback"),
                Items =
        {
            { "returnUrl", returnUrl },
            { "scheme", GoogleDefaults.AuthenticationScheme }
        }
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (result?.Succeeded != true)
            {
                return RedirectToAction("Login");
            }

            var returnUrl = result.Properties?.GetString("returnUrl") ?? Url.Content("~/");

            // معالجة المستخدم وتسجيل الدخول
            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                TempData["Email"] = email;
                return RedirectToAction("PreRegister", "Account");
            }

            await signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToLocal(returnUrl);
        }
        #endregion

        #region Change Password
        //Change Password
        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost("ChangePassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userService.ChangePasswordAsync(user.Id, model.CurrentPassword, model.NewPassword);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Error changing password. Please check your current password.");
                return View(model);
            }

            await signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Your password has been changed successfully.";
            return RedirectToAction(nameof(ChangePassword));
        }
        #endregion

        #region Logout
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        #endregion

        #region Validations

        public async Task<ActionResult> IsUniqueEmail(string Email)
        {

                var user = await userManager.FindByEmailAsync(Email);
                if (user == null)
                    return Json(true);
                else
                    return Json(false);

        }

        public async Task<IActionResult> IsUniqueUserName(string Username)
        {
                var user = await userManager.FindByNameAsync(Username);
                if (user == null)
                    return Json(true);
                else
                    return Json(false);
        }

        #endregion 
    }
}
