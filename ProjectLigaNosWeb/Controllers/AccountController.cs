using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectLigaNosWeb.Data.Entities;
using ProjectLigaNosWeb.Helpers;
using ProjectLigaNosWeb.Models;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;

namespace ProjectLigaNosWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _emailHelper;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;



        public AccountController(IUserHelper userHelper, IMailHelper emailHelper, IConfiguration configuration, UserManager<User> userManager, IWebHostEnvironment hostingEnvironment, SignInManager<User> signInManager)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _emailHelper = emailHelper;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Username);
                    if (user != null)
                    {
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName), 
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("ProfilePicture", string.IsNullOrEmpty(user.ProfilePicture) ? "default_profile_picture_url" : user.ProfilePicture)
                };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe 
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        
                        if (this.Request.Query.Keys.Contains("ReturnUrl"))
                        {
                            return Redirect(this.Request.Query["ReturnUrl"].First());
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "Failed. Try again!");
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            ViewBag.Roles = new List<string> { "Admin", "Funcionario", "Clube" };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user == null)
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        PostalCode = model.PostalCode
                    };

                    if (model.ProfilePicture != null)
                    {
                        var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images/users");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfilePicture.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        Directory.CreateDirectory(uploadsFolder);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ProfilePicture.CopyToAsync(fileStream);
                        }

                        user.ProfilePicture = "/images/users/" + uniqueFileName;
                    }

                    var result = await _userHelper.AddUserAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.Role))
                        {
                            await _userHelper.AddUserToRoleAsync(user, model.Role);
                        }
                        else
                        {
                            await _userHelper.AddUserToRoleAsync(user, "Funcionario");
                        }

                        string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                        string tokenLink = Url.Action("ConfirmEmail", "Account", new
                        {
                            userid = user.Id,
                            token = myToken
                        }, protocol: HttpContext.Request.Scheme);

                        _emailHelper.SendEmail(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                            $"To confirm your account, please click on this link:</br></br><a href=\"{tokenLink}\"> Confirm Email</a>");

                        TempData["SuccessMessage"] = "The confirmation email has been sent. Please check your inbox.";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The user already exists.");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            var model = new ChangeUserViewModel();

            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Address = user.Address;
                model.PostalCode = user.PostalCode;
                model.PhoneNumber = user.PhoneNumber;
                model.ProfilePicturePath = user.ProfilePicture;
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Funcionario, Clube")]

        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;

                    if (model.ProfilePicture != null)
                    {
                        var imagesFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images/users");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfilePicture.FileName;
                        var filePath = Path.Combine(imagesFolder, uniqueFileName);

                        Directory.CreateDirectory(imagesFolder);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ProfilePicture.CopyToAsync(fileStream);
                        }

                        user.ProfilePicture = "/images/users/" + uniqueFileName;
                    }

                    await _userHelper.UpdateUserAsync(user);

                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("ProfilePicture", user.ProfilePicture) 
            };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }
            }

            return View(model);
        }


        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Funcionario, Clube")]

        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return this.RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "User not found.");
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(
                   user,
                   model.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                         new Claim("ProfilePicture", "/images/users/" + user.ProfilePicture)

                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15),
                            signingCredentials: credentials);

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return this.Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {

            }

            return View();
        }

        [HttpGet]
        [Route("Account/RecoverPassword")]

        public IActionResult RecoverPassword()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't correspont to a registered user.");
                    return View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                var link = this.Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);

                Response response = _emailHelper.SendEmail(model.Email, "Shop Password Reset", $"<h1>Shop Password Reset</h1>" +
                $"To reset the password click in this link:</br></br>" +
                $"<a href = \"{link}\">Reset Password</a>");

                if (response.IsSuccess)
                {
                    this.ViewBag.Message = "The instructions to recover your password has been sent to email.";
                }

                return this.View();

            }

            return this.View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.UserName);
            if (user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    this.ViewBag.Message = "Password reset successful.";
                    return View();
                }

                this.ViewBag.Message = "Error while resetting the password.";
                return View(model);
            }

            this.ViewBag.Message = "User not found.";
            return View(model);
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            var profilePicturePath = Path.Combine(_hostingEnvironment.WebRootPath, "images/users", user.UserName + ".png");

            if (!System.IO.File.Exists(profilePicturePath))
            {
                ViewBag.ProfilePicturePath = "/images/users/default.png";
            }
            else
            {
                var timestamp = System.IO.File.GetLastWriteTime(profilePicturePath).Ticks; 
                ViewBag.ProfilePicturePath = $"/images/users/{user.UserName}.png?t={timestamp}";
            }


            return View();
        }


        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}
