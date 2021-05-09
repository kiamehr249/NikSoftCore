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

        [HttpGet, AllowAnonymous]
        public IActionResult Register([FromQuery] string lang)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/Panel");
            }

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            ViewBag.Messages = Messages;
            UserRegisterRequest model = new UserRegisterRequest();
            return View(GetViewName(lang, "Register"), model);
        }


        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register([FromQuery] string lang, UserRegisterRequest request)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/Panel");
            }

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (string.IsNullOrEmpty(request.Firstname))
            {
                AddError("نام باید مقدار داشته باشد", "fa");
            }

            if (string.IsNullOrEmpty(request.Lastname))
            {
                AddError("نام خانوادگی باید مقدار داشته باشد", "fa");
            }

            if (string.IsNullOrEmpty(request.Email))
            {
                if (lang == "fa")
                    AddError("آدرس ایمیل باید مقدار داشته باشد", "fa");
                else
                    AddError("Email can not be null", "en");
            }

            if (string.IsNullOrEmpty(request.PhoneNumber))
            {
                if (lang == "fa")
                    AddError("شماره موبایل باید مقدار داشته باشد", "fa");
                else
                    AddError("Mobile can not be null", "en");
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                if (lang == "fa")
                    AddError("رمز عبور باید مقدار داشته باشد", "fa");
                else
                    AddError("Password can not be null", "en");
            }

            if (string.IsNullOrEmpty(request.ConfirmPassword))
            {
                if (lang == "fa")
                    AddError("تکرار رمز عبور باید مقدار داشته باشد", "fa");
                else
                    AddError("Confirm password can not be null", "en");
            }

            if (request.Password != request.ConfirmPassword)
            {
                if (lang == "fa")
                    AddError("تکرار رمز عبور با رمز عبور مطابق نیست", "fa");
                else
                    AddError("Password and confirm password is not same", "en");
            }

            ViewBag.Messages = Messages;

            if (Messages.Count(x => x.Type == ViewModel.MessageType.Error) > 0)
            {
                return View(GetViewName(lang, "Register"), request);
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
                        return View(GetViewName(lang, "Register"), request);
                    }
                }
                else
                {
                    if (lang == "fa")
                    {
                        AddError("این کاربری در سامانه موجود است", "fa");
                    }
                    else
                    {
                        AddError("This user already exist", "en");
                    }

                    ViewBag.Messages = Messages;
                    //ModelState.AddModelError("message", "Email already exists.");
                    return View(GetViewName(lang, "Register"), request);
                }
            }
            return View(GetViewName(lang, "Register"), request);

        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login([FromQuery] string lang)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/Panel");
            }

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            LoginRequest model = new LoginRequest();
            return View(GetViewName(lang, "Login"), model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromQuery] string lang, LoginRequest model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/Panel");
            }

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (string.IsNullOrEmpty(model.Username))
            {
                AddError("نام کاربری را وارد کنید", "fa");
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                AddError("رمز عبور را وارد کنید", "fa");
            }

            if (Messages.Count > 0)
            {
                ViewBag.Messages = Messages;
                return View(GetViewName(lang, "Login"), model);
            }

            model.Username = model.Username.PersianToEnglish();

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Username);
                if (user != null && !user.EmailConfirmed)
                {
                    if (lang == "fa")
                    {
                        AddError("این نام کابری هنوز تایید نشده است", "fa");
                        //ModelState.AddModelError("message", "این نام کابری هنوز تایید نشده است");
                    }
                    else
                    {
                        AddError("Email not confirmed yet", "en");
                        //ModelState.AddModelError("message", "Email not confirmed yet");
                    }

                    ViewBag.Messages = Messages;
                    return View(GetViewName(lang, "Login"), model);

                }

                if (user == null)
                {
                    user = await userManager.FindByNameAsync(model.Username);
                }

                if (await userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    if (lang == "fa")
                    {
                        AddError("این کاربری نا معتبر است", "fa");
                        //ModelState.AddModelError("message", "این کاربری نا معتبر است");
                    }
                    else
                    {
                        AddError("Invalid credentials", "en");
                        //ModelState.AddModelError("message", "Invalid credentials");
                    }

                    ViewBag.Messages = Messages;
                    return View(GetViewName(lang, "Login"), model);

                }

                var hasUserRole = await userManager.IsInRoleAsync(user, "User");
                if (!hasUserRole)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }

                var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    if (user.AccountType == AccountType.Merchant)
                    {
                        return Redirect("/Home/Profile");
                    }

                    return Redirect("/Panel");
                }
                else if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                else
                {
                    if (lang == "fa")
                    {
                        AddError("ورود نا معتبر", "fa");
                        //ModelState.AddModelError("message", "ورود نا معتبر");
                    }
                    else
                    {
                        AddError("ورود نا معتبر", "fa");
                        //ModelState.AddModelError("message", "Invalid login attempt");
                    }
                    ViewBag.Messages = Messages;
                    return View(GetViewName(lang, "Login"), model);
                }
            }
            return View(GetViewName(lang, "Login"), model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Redirect("/Auth/Account/Login");
        }



    }
}
