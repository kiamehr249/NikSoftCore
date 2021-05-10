using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.SystemBase.Service;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
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
        public async Task<IActionResult> Index()
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theProfile = await ISystemBaseServ.iUserProfileServ.FindAsync(x => x.UserId == theUser.Id);

            ViewBag.PageTitle = "مدیریت پروفایل";

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

            return View(profileRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] UserProfileRequest request)
        {

            ViewBag.PageTitle = "ایجاد دسته بندی";


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
                    return View(request);
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
                    View();
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


            return View();
        }


    }
}
