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
using HadafServices;

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

        [HttpGet, Authorize]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/Panel");
            }

            ViewBag.Messages = Messages;
            UserRegisterRequest model = new UserRegisterRequest();
            return View(model);
        }


        [HttpPost, Authorize]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/Panel");
            }

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
                AddError("آدرس ایمیل باید مقدار داشته باشد", "fa");
            }

            if (string.IsNullOrEmpty(request.PhoneNumber))
            {
                AddError("شماره موبایل باید مقدار داشته باشد", "fa");
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                AddError("رمز عبور باید مقدار داشته باشد", "fa");
            }

            if (string.IsNullOrEmpty(request.ConfirmPassword))
            {
                AddError("تکرار رمز عبور باید مقدار داشته باشد", "fa");
            }

            if (request.Password != request.ConfirmPassword)
            {
                AddError("تکرار رمز عبور با رمز عبور مطابق نیست", "fa");
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
        public async Task<IActionResult> Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/Panel");
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
                return Redirect("/Panel");
            }

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
                return View(model);
            }

            model.Username = model.Username.PersianToEnglish();

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Username);
                if (user != null && !user.EmailConfirmed)
                {
                    AddError("این نام کابری هنوز تایید نشده است", "fa");

                    ViewBag.Messages = Messages;
                    return View(model);

                }

                if (user == null)
                {
                    user = await userManager.FindByNameAsync(model.Username);
                }

                if (await userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    AddError("نام کاربری یا رمز عبور را اشتباه وارد کرده‌اید", "fa");

                    ViewBag.Messages = Messages;
                    return View(model);

                }

                //var isAdmin = await userManager.IsInRoleAsync(user, "NikAdmin");
                //if (!isAdmin)
                //{
                //    var authExt = await CheckAuthService(user.UserName, user.PhoneNumber);
                //    if (authExt == "false")
                //    {
                //        AddError("کاربری شما نا معتبر است.", "fa");
                //        ViewBag.Messages = Messages;
                //        return View(model);
                //    }
                //}
                

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
                    AddError("ورود نا معتبر", "fa");
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


        private async Task<string> CheckAuthService(string NCode, string Mobile, string Comment = "")
        {
            HadafServicesSoapClient service = new HadafServicesSoapClient(HadafServicesSoapClient.EndpointConfiguration.HadafServicesSoap);
            var token = await service.GetTokenAsync();
            var authKey = new AuthenticationKey();
            var key = authKey.GenerateKey(NCode, Mobile, token.Body.GetTokenResult, Comment);
            var authRes = await service.AuthenticateAsync(key);
            return authRes.Body.AuthenticateResult;
        }

    }
}
