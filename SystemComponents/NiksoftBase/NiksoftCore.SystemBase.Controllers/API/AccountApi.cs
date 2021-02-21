using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.DataModel;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.SystemBase.Service;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel.User;
using System.Threading.Tasks;

namespace NiksoftCore.SystemBase.Controllers.API
{
    [Route("/api/base/[controller]/[action]")]
    public class AccountApi : NikApi
    {
        public IConfiguration Configuration { get; }
        public ISystemBaseService iSystemBaseServ { get; set; }
        public User theUser { get; set; }

        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AccountApi(IConfiguration configuration, UserManager<DataModel.User> userManager, SignInManager<User> signInManager)
        {
            Configuration = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
            iSystemBaseServ = new SystemBaseService(Configuration.GetConnectionString("SystemBase"));
            GetUserAsync();
        }

        [HttpPost]
        public async Task<IActionResult> SignInUser([FromForm] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return Ok(new
                {
                    status = 500,
                    message = "خطا در مقادیر ورودی"
                });
            }

            request.Username = request.Username.PersianToEnglish();

            var user = await userManager.FindByEmailAsync(request.Username);
            if (user != null && !user.EmailConfirmed)
            {
                return Ok(new
                {
                    status = 402,
                    message = "این نام کابری هنوز تایید نشده است"
                });

            }

            if (user == null)
            {
                user = await userManager.FindByNameAsync(request.Username);
            }

            if (await userManager.CheckPasswordAsync(user, request.Password) == false)
            {

                return Ok(new
                {
                    status = 402,
                    message = "این کاربری نا معتبر است"
                });
            }

            var result = await signInManager.PasswordSignInAsync(request.Username, request.Password, request.RememberMe, true);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    status = 200,
                    message = "ورود موفق",
                    data = true
                });
            }
            else if (result.IsLockedOut)
            {
                return Ok(new
                {
                    status = 403,
                    message = "عدم دسترسی به سامانه",
                    data = false
                });
            }
            else
            {
                return Ok(new
                {
                    status = 403,
                    message = "عدم دسترسی به سامانه",
                    data = false
                });
            }
        }


        private async Task GetUserAsync()
        {
            theUser = await userManager.GetUserAsync(HttpContext.User);
        }

    }
}
