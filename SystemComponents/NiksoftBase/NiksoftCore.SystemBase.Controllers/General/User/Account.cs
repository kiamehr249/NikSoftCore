using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.DataModel;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.SystemBase.Service;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel.User;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HadafAuthentication;

namespace NiksoftCore.SystemBase.Controllers.General.User
{
    [Area("Auth")]
    public class Account : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly SignInManager<DataModel.User> signInManager;

        public Account(UserManager<DataModel.User> userManager, SignInManager<DataModel.User> signInManager, IConfiguration Configuration) : base(Configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                var theUser = await userManager.GetUserAsync(HttpContext.User);
                if (User.IsInRole("Admin"))
                {
                    return Redirect("/Panel");
                }
                else
                {
                    return Redirect("/Dashboard");
                }

            }

            ViewBag.Messages = Messages;
            UserRegisterRequest model = new UserRegisterRequest();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("NikAdmin") || User.IsInRole("Admin"))
                {
                    return Redirect("/Panel");
                }
                else
                {
                    return Redirect("/Dashboard");
                }

            }

            if (string.IsNullOrEmpty(request.Firstname))
            {
                AddError("The name must have a value", "fa");
            }

            if (string.IsNullOrEmpty(request.Lastname))
            {
                AddError("Last name must have a value", "fa");
            }

            if (string.IsNullOrEmpty(request.Email))
            {
                AddError("Email address must have a value", "fa");
            }

            if (string.IsNullOrEmpty(request.PhoneNumber))
            {
                AddError("The mobile number must have a value", "fa");
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                AddError("Password must have a value ", "fa");
            }

            if (string.IsNullOrEmpty(request.ConfirmPassword))
            {
                AddError("Repeat password must have a value", "fa");
            }

            if (request.Password != request.ConfirmPassword)
            {
                AddError("Repeat password does not match password", "fa");
            }

            ViewBag.Messages = Messages;

            if (Messages.Count(x => x.Type == ViewModel.MessageType.Error) > 0)
            {
                return View(request);
            }

            request.PhoneNumber = request.PhoneNumber.PersianToEnglish();

            if (ModelState.IsValid)
            {
                var userCheck = await userManager.FindByNameAsync(request.PhoneNumber);
                if (userCheck == null)
                {
                    var user = new DataModel.User
                    {
                        UserName = request.PhoneNumber,
                        NormalizedUserName = request.PhoneNumber,
                        Email = request.Email,
                        PhoneNumber = request.PhoneNumber,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                    };
                    var result = await userManager.CreateAsync(user, request.Password);
                    if (result.Succeeded)
                    {
                        var profile = new UserProfile
                        {
                            Firstname = request.Firstname,
                            Lastname = request.Lastname,
                            UserId = user.Id
                        };
                        ISystemBaseServ.iUserProfileServ.Add(profile);
                        await ISystemBaseServ.iUserProfileServ.SaveChangesAsync();
                        return Redirect("/Auth/Account/Login");
                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                        {
                            foreach (var error in result.Errors)
                            {
                                AddError("message" + error.Description, "en");
                                //ModelState.AddModelError("message", error.Description);
                            }
                        }
                        ViewBag.Messages = Messages;
                        return View(request);
                    }
                }
                else
                {
                    AddError("این کاربری در سامانه موجود است", "fa");

                    ViewBag.Messages = Messages;
                    //ModelState.AddModelError("message", "Email already exists.");
                    return View(request);
                }
            }
            return View(request);

        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("NikAdmin") || User.IsInRole("Admin"))
                {
                    return Redirect("/Panel");
                }
                else
                {
                    return Redirect("/Dashboard");
                }
            }

            LoginRequest model = new LoginRequest();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("NikAdmin") || User.IsInRole("Admin"))
                {
                    return Redirect("/Panel");
                }
                else
                {
                    return Redirect("/Dashboard");
                }
            }

            if (string.IsNullOrEmpty(model.Username))
            {
                AddError("Enter the username", "fa");
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                AddError("Enter your password", "fa");
            }

            if (Messages.Count > 0)
            {
                ViewBag.Messages = Messages;
                return View(model);
            }

            model.Username = model.Username.PersianToEnglish();

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Username);
                if (user != null && !user.EmailConfirmed)
                {
                    AddError("This username has not been verified yet", "fa");

                    ViewBag.Messages = Messages;
                    return View(model);

                }

                if (user == null)
                {
                    user = await userManager.FindByNameAsync(model.Username);
                }

                if (await userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    AddError("You entered the wrong username or password", "fa");

                    ViewBag.Messages = Messages;
                    return View(model);

                }


                var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    var IsNik = await userManager.IsInRoleAsync(user, "NikAdmin");
                    var IsAdmin = await userManager.IsInRoleAsync(user, "Admin");
                    if (IsNik || IsAdmin)
                    {
                        return Redirect("/Panel");
                    }
                    else
                    {
                        return Redirect("/Dashboard");
                    }
                }
                else if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                else
                {
                    AddError("Invalid login", "fa");
                    ViewBag.Messages = Messages;
                    return View(model);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Redirect("/Auth/Account/Login");
        }

    }
}
