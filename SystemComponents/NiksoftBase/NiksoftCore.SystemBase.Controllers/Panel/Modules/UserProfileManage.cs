using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.SystemBase.Service;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System.Collections.Generic;
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

        //public async Task<IActionResult> Index()
        //{
        //    return View();
        //}


        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            ViewBag.Messages = Messages;
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theProfile = await ISystemBaseServ.iUserProfileServ.FindAsync(x => x.UserId == theUser.Id);

            ViewBag.PageTitle = "اطلاعات کاربری";

            var profileRequest = new UserProfileRequest();

            if (theProfile != null)
            {
                profileRequest.Id = theProfile.Id;
                profileRequest.Firstname = theProfile.Firstname;
                profileRequest.Lastname = theProfile.Lastname;
                profileRequest.NCode = theProfile.NCode;
                profileRequest.Mobile = theProfile.Mobile;
                profileRequest.Tel = theProfile.Tel;
                profileRequest.Address = theProfile.Address;
                profileRequest.ZipCode = theProfile.ZipCode;
                profileRequest.BirthDate = theProfile.BirthDate != null ? theProfile.BirthDate.Value.ToPersianDateTime().ToPersianDigitalDateString() : "";
                profileRequest.UserId = theProfile.UserId;
                profileRequest.Avatar = theProfile.Avatar;
                profileRequest.NCardImage = theProfile.NCardImage;
                profileRequest.IdCardImage = theProfile.IdCardImage;
                profileRequest.ProvinceId = theProfile.ProvinceId ?? 0;
                profileRequest.CityId = theProfile.CityId ?? 0;
                profileRequest.Gender = theProfile.Gender ?? 0;
            }
            else
            {
                profileRequest.Mobile = theUser.PhoneNumber;
                profileRequest.UserId = theUser.Id;
            }
            DropDownBinder(profileRequest.ProvinceId, profileRequest.Gender);
            return View(profileRequest);
        }

        [HttpPost]
        public async Task<IActionResult> MyProfile(UserProfileRequest request)
        {
            ViewBag.PageTitle = "اطلاعات کاربری";


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
                    AddError("آپلود فایل انجام نشد مجدد تلاش کنید");
                    ViewBag.Messages = Messages;
                    DropDownBinder(request.ProvinceId, request.Gender);
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
            thisItem.NCode = request.NCode;
            thisItem.Mobile = request.Mobile;
            thisItem.Tel = request.Tel;
            thisItem.Address = request.Address;
            thisItem.ZipCode = request.ZipCode;
            thisItem.BirthDate = !string.IsNullOrEmpty(request.BirthDate) ? PersianDateTime.Parse(request.BirthDate).ToDateTime() : null;
            thisItem.UserId = request.UserId;
            thisItem.CityId = request.CityId > 0 ? request.CityId : null;
            thisItem.ProvinceId = request.ProvinceId > 0 ? request.ProvinceId : null;
            thisItem.Gender = request.Gender > 0 ? request.Gender : null;

            if (!string.IsNullOrEmpty(avatar))
                thisItem.Avatar = avatar;

            if (request.Id == 0)
            {
                thisItem.Status = ProfileStatus.Save;
                ISystemBaseServ.iUserProfileServ.Add(thisItem);
            }

            await ISystemBaseServ.iUserProfileServ.SaveChangesAsync();

            DropDownBinder(request.ProvinceId, request.Gender);

            AddSuccess("اطلاعات با موفقیت ثبت شد");
            ViewBag.Messages = Messages;

            return View(request);
        }

        private void DropDownBinder(int provinceId, int genderId)
        {
            var provinces = ISystemBaseServ.iProvinceServ.GetAll(x => true);
            ViewBag.Provinces = new SelectList(provinces, "Id", "Title", provinceId);

            List<ListItemModel> genders = new List<ListItemModel>();
            genders.Add(new ListItemModel { 
                Id = 1,
                Title = "مرد"
            });
            genders.Add(new ListItemModel
            {
                Id = 2,
                Title = "زن"
            });
            ViewBag.Genders = new SelectList(genders, "Id", "Title", genderId);
        }
    }
}
