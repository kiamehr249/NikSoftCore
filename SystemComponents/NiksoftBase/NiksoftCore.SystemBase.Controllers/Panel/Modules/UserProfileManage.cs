using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.SystemBase.Service;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.SystemBase.Controllers.Panel.Modules
{
    [Area("Panel")]
    public class UserProfileManage : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly RoleManager<DataModel.Role> roleManager;
        private readonly IWebHostEnvironment hosting;

        public UserProfileManage(IConfiguration Configuration,
            UserManager<DataModel.User> userManager,
            IWebHostEnvironment hosting,
            RoleManager<DataModel.Role> roleManager) : base(Configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.hosting = hosting;
        }

        [HttpGet]
        public async Task<IActionResult> Index(BaseRequest request)
        {
            if (!string.IsNullOrEmpty(request.lang))
                request.lang = request.lang.ToLower();
            else
                request.lang = defaultLang.ShortName.ToLower();

            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theProfile = await ISystemBaseServ.iUserProfileServ.FindAsync(x => x.UserId == theUser.Id);

            if (request.lang == "fa")
                ViewBag.PageTitle = "مدیریت پروفایل";
            else
                ViewBag.PageTitle = "Profile Management";

            var profileRequest = new UserProfileRequest();

            if (theProfile != null)
            {
                profileRequest.Id = theProfile.Id;
                profileRequest.Firstname = theProfile.Firstname;
                profileRequest.Lastname = theProfile.Lastname;
                profileRequest.Mobile = theProfile.Mobile;
                profileRequest.Tel = theProfile.Tel;
                profileRequest.Address = theProfile.Address;
                profileRequest.ZipCode = theProfile.ZipCode;
                profileRequest.BirthDate = theProfile.BirthDate;
                profileRequest.UserId = theProfile.UserId;
                profileRequest.Avatar = theProfile.Avatar;
            }
            else
            {
                profileRequest.Mobile = theUser.PhoneNumber;
                profileRequest.UserId = theUser.Id;
            }

            return View(GetViewName(request.lang, "Index"), profileRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromQuery] string lang, [FromForm] UserProfileRequest request)
        {
            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (lang == "fa")
                ViewBag.PageTitle = "ایجاد دسته بندی";
            else
                ViewBag.PageTitle = "Create Business Category";


            string avatar = string.Empty;
            if (request.AvatarFile != null && request.AvatarFile.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.AvatarFile,
                    RootPath = hosting.ContentRootPath,
                    UnitPath = Config.GetSection("FileRoot:BaseSystem").Value
                });

                if (!SaveImage.Success)
                {
                    Messages.Add(new NikMessage
                    {
                        Message = "آپلود فایل انجام نشد مجدد تلاش کنید",
                        Type = MessageType.Error,
                        Language = "Fa"
                    });
                    ViewBag.Messages = Messages;
                    return View(GetViewName(lang, "Create"), request);
                }

                avatar = SaveImage.FilePath;
            }


            UserProfile thisItem;

            if (request.Id > 0)
            {
                thisItem = await ISystemBaseServ.iUserProfileServ.FindAsync(x => x.Id == request.Id);
            }
            else
            {
                var readyProfile = await ISystemBaseServ.iUserProfileServ.FindAsync(x => x.UserId == request.UserId);
                if (readyProfile != null)
                {
                    View(GetViewName(lang, "Index"));
                }
                thisItem = new UserProfile();
            }




            thisItem.Firstname = request.Firstname;
            thisItem.Lastname = request.Lastname;
            thisItem.Mobile = request.Mobile;
            thisItem.Tel = request.Tel;
            thisItem.Address = request.Address;
            thisItem.ZipCode = request.ZipCode;
            thisItem.BirthDate = request.BirthDate;
            thisItem.UserId = request.UserId;
            if (!string.IsNullOrEmpty(avatar))
            {
                thisItem.Avatar = avatar;
            }


            if (request.Id == 0)
            {
                ISystemBaseServ.iUserProfileServ.Add(thisItem);
            }

            await ISystemBaseServ.iUserProfileServ.SaveChangesAsync();


            return View(GetViewName(lang, "Index"));
        }


    }
}
