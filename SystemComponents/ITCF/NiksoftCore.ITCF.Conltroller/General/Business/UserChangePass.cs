using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.ITCF.Conltroller.General.Business
{
    [Area("Business")]
    [Authorize]
    public class UserChangePass : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly RoleManager<DataModel.Role> roleManager;
        private readonly IWebHostEnvironment hosting;

        public UserChangePass(IConfiguration Configuration,
            UserManager<DataModel.User> userManager,
            IWebHostEnvironment hosting,
            RoleManager<DataModel.Role> roleManager) : base(Configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.hosting = hosting;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theProfile = await ISystemBaseServ.iUserProfileServ.FindAsync(x => x.UserId == theUser.Id);

            ViewBag.PageTitle = "تغییر رمز عبور";
            var request = new ChangePassRequest();
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] ChangePassRequest request)
        {

            ViewBag.PageTitle = "پروفایل کاربری";
            ViewBag.Messages = Messages;
            if (!ValidForm(request))
            {
                return View(request);
            }

            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theProfile = await ISystemBaseServ.iUserProfileServ.FindAsync(x => x.UserId == theUser.Id);

            var checkPass = await userManager.CheckPasswordAsync(theUser, request.OldPassword);
            if (!checkPass)
            {
                Messages.Add(new NikMessage
                {
                    Message = "پسورد فعلی را صحیح وارد کنید",
                    Language = "fa",
                    Type = MessageType.Error
                });
                ViewBag.Messages = Messages;
                return View(request);
            }

            var changed = await userManager.ChangePasswordAsync(theUser, request.OldPassword, request.NewPassword);
            if (!changed.Succeeded)
            {
                Messages.Add(new NikMessage
                {
                    Message = "تغییر رمز عبور انجام نشد",
                    Language = "fa",
                    Type = MessageType.Error
                });
                ViewBag.Messages = Messages;
                return View(request);
            }

            return View();
        }

        public bool ValidForm(ChangePassRequest request)
        {
            if (string.IsNullOrEmpty(request.OldPassword))
            {
                Messages.Add(new NikMessage { 
                    Message = "رمز عبور قبلی را وارد کنید",
                    Language = "fa",
                    Type = MessageType.Error
                });
            }

            if (string.IsNullOrEmpty(request.NewPassword))
            {
                Messages.Add(new NikMessage
                {
                    Message = "رمز عبور جدید را وارد کنید",
                    Language = "fa",
                    Type = MessageType.Error
                });
            }

            if (string.IsNullOrEmpty(request.ConfirmPass))
            {
                Messages.Add(new NikMessage
                {
                    Message = "تکرار رمز عبور جدید را وارد کنید",
                    Language = "fa",
                    Type = MessageType.Error
                });
            }

            if (request.NewPassword != request.ConfirmPass)
            {
                Messages.Add(new NikMessage
                {
                    Message = "رمز عبور جدید و تکرار آن یکسان نیست",
                    Language = "fa",
                    Type = MessageType.Error
                });
            }

            if (Messages.Count > 0)
            {
                return false;
            }

            return true;
        }


    }
}
