using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.Bourse.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.Bourse.Controllers.Panel
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin,Admin")]
    public class BUserManager : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly RoleManager<DataModel.Role> roleManager;
        public IBourseService iBourseServ { get; set; }
        private readonly IWebHostEnvironment hosting;

        public BUserManager(IConfiguration Configuration,
            IWebHostEnvironment hosting,
            UserManager<DataModel.User> userManager,
            RoleManager<DataModel.Role> roleManager) : base(Configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.hosting = hosting;
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
        }

        public IActionResult Index(UserGridRequest request)
        {
            var query = ISystemBaseServ.iNikUserServ.ExpressionMaker();
            query.Add(x => true);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.UserName))
            {
                query.Add(x => x.UserName.Contains(request.UserName));
                isSearch = true;
            }

            if (!string.IsNullOrEmpty(request.FirstName))
            {
                var userIds = ISystemBaseServ.iUserProfileServ.GetAll(x => x.Firstname.Contains(request.FirstName)).Select(x => x.UserId);
                query.Add(x => userIds.Contains(x.Id));
                isSearch = true;
            }

            if (!string.IsNullOrEmpty(request.LastName))
            {
                var userIds = ISystemBaseServ.iUserProfileServ.GetAll(x => x.Lastname.Contains(request.LastName)).Select(x => x.UserId);
                query.Add(x => userIds.Contains(x.Id));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = ISystemBaseServ.iNikUserServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;


            ViewBag.PageTitle = "مدیریت کاربران";

            ViewBag.Contents = ISystemBaseServ.iNikUserServ.GetPart(query, pager.StartIndex, pager.PageSize, x => x.Id, true).ToList();

            return View();
        }

        [HttpGet]
        public IActionResult Create(int Id)
        {
            ViewBag.PageTitle = "ایجاد کاربر";

            var request = new BourseUserRequest();

            if (Id > 0)
            {
                var theUser = ISystemBaseServ.iNikUserServ.Find(x => x.Id == Id);
                request.Id = theUser.Id;
                request.Email = theUser.Email;

                var theProfile = ISystemBaseServ.iUserProfileServ.Find(x => x.UserId == theUser.Id);
                if (theProfile != null)
                {
                    request.ProfileId = theProfile.Id;
                    request.Firstname = theProfile.Firstname;
                    request.Lastname = theProfile.Lastname;
                    request.NCode = theProfile.NCode;
                    request.Mobile = theProfile.Mobile;
                    request.Tel = theProfile.Tel;
                    request.Address = theProfile.Address;
                    request.ZipCode = theProfile.ZipCode;
                    request.BirthDate = theProfile.BirthDate != null ? theProfile.BirthDate.Value.ToPersianDateTime().ToPersianDigitalDateString() : "";
                    request.Avatar = theProfile.Avatar;
                    request.IdCardImage = theProfile.IdCardImage;
                    request.NCardImage = theProfile.NCardImage;
                    request.ProvinceId = theProfile.ProvinceId ?? 0;
                    request.CityId = theProfile.CityId ?? 0;
                    request.Gender = theProfile.Gender ?? 0;
                }

                var theBank = iBourseServ.iUserBankAccountServ.Find(x => x.UserId == theUser.Id);
                if (theBank != null)
                {
                    request.BankId = theBank.Id;
                    request.PAN = theBank.PAN;
                    request.IBAN = theBank.IBAN;
                    request.BankName = theBank.BankName;
                    request.BranchName = theBank.BranchName;
                    request.BranchCode = theBank.BranchCode;
                }
            }

            DropDownBinder(request.ProvinceId, request.Gender);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BourseUserRequest request)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (!ValidUserForm(request))
            {
                ViewBag.Messages = Messages;
                DropDownBinder(request.ProvinceId, request.Gender);
                return View(request);
            }

            DataModel.User item = new DataModel.User();
            if (request.Id > 0)
            {
                item = await userManager.FindByIdAsync(request.Id.ToString());
            }


            item.UserName = request.NCode;
            item.Email = request.Email;
            item.PhoneNumber = request.Mobile;


            if (request.Id == 0)
            {
                item.EmailConfirmed = true;
                item.PhoneNumberConfirmed = true;
                await userManager.CreateAsync(item, request.Password);
            }
            else
            {

                if (!string.IsNullOrEmpty(request.Password) && !string.IsNullOrEmpty(request.ConfirmedPassword))
                {
                    var passHash = userManager.PasswordHasher.HashPassword(item, request.Password);
                    item.PasswordHash = passHash;
                }

                await userManager.UpdateAsync(item);

            }

            SystemBase.Service.UserProfile profile = new SystemBase.Service.UserProfile();
            if (request.ProfileId > 0)
            {
                profile = ISystemBaseServ.iUserProfileServ.Find(x => x.UserId == item.Id);
            }

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

            string nCardImage = string.Empty;
            if (request.NCardFile != null && request.NCardFile.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.NCardFile,
                    RootPath = hosting.ContentRootPath,
                    UnitPath = Config.GetSection("FileRoot:MarketerFiles").Value
                });

                if (!SaveImage.Success)
                {
                    AddError("آپلود فایل انجام نشد مجدد تلاش کنید");
                    ViewBag.Messages = Messages;
                    DropDownBinder(request.ProvinceId, request.Gender);
                    return View(request);
                }

                nCardImage = SaveImage.FilePath;
            }

            string IdCardImage = string.Empty;
            if (request.IdCardFile != null && request.IdCardFile.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.IdCardFile,
                    RootPath = hosting.ContentRootPath,
                    UnitPath = Config.GetSection("FileRoot:MarketerFiles").Value
                });

                if (!SaveImage.Success)
                {
                    AddError("آپلود فایل انجام نشد مجدد تلاش کنید");
                    ViewBag.Messages = Messages;
                    DropDownBinder(request.ProvinceId, request.Gender);
                    return View(request);
                }
                IdCardImage = SaveImage.FilePath;
            }

            profile.Firstname = request.Firstname;
            profile.Lastname = request.Lastname;
            profile.NCode = request.NCode;
            profile.CompanyName = request.CompanyName;
            profile.Mobile = request.Mobile;
            profile.Tel = request.Tel;
            profile.Address = request.Address;
            profile.ZipCode = request.ZipCode;
            profile.BirthDate = PersianDateTime.Parse(request.BirthDate).ToDateTime();
            if (!string.IsNullOrEmpty(avatar))
                profile.Avatar = avatar;
            if (!string.IsNullOrEmpty(nCardImage))
                profile.NCardImage = nCardImage;
            if (!string.IsNullOrEmpty(IdCardImage))
                profile.IdCardImage = IdCardImage;
            profile.ProvinceId = request.ProvinceId == 0 ? null : request.ProvinceId;
            profile.CityId = request.CityId == 0 ? null : request.CityId;
            profile.Gender = request.Gender;

            if (request.ProfileId == 0)
            {
                profile.UserId = item.Id;
                profile.Status = ProfileStatus.Save;
                ISystemBaseServ.iUserProfileServ.Add(profile);
            }

            await ISystemBaseServ.iUserProfileServ.SaveChangesAsync();

            UserBankAccount bank = new UserBankAccount();
            if (request.BankId > 0)
            {
                bank = iBourseServ.iUserBankAccountServ.Find(x => x.Id == request.BankId);
                bank.EditDate = DateTime.Now;
                bank.EditedBy = user.Id;
            }

            bank.PAN = request.PAN;
            bank.IBAN = request.IBAN;
            bank.BankName = request.BankName;
            bank.BranchName = request.BranchName;
            bank.BranchCode = request.BranchCode;


            if (request.BankId == 0)
            {
                bank.UserId = item.Id;
                bank.CreateDate = DateTime.Now;
                bank.CreatedBy = user.Id;
                iBourseServ.iUserBankAccountServ.Add(bank);
            }

            await iBourseServ.iUserBankAccountServ.SaveChangesAsync();

            return Redirect("/Panel/BUserManager");

        }

        public async Task<IActionResult> Remove(int Id)
        {
            var user = await userManager.FindByIdAsync(Id.ToString());
            try
            {
                await userManager.DeleteAsync(user);
            }
            catch
            {

            }

            return Redirect("/Panel/BUserManager");
        }

        public bool ValidUserForm(BourseUserRequest request)
        {
            bool result = true;
            if (request.Id == 0)
            {
                var readyUser = ISystemBaseServ.iNikUserServ.Find(x => x.UserName == request.NCode || x.Email == request.Email);
                if (readyUser != null)
                {
                    AddError("کاربری با این کد ملی یا ایمیل قبلا ثبت شده است", "fa");
                    result = false;
                }
            }
            else
            {
                var readyUser = ISystemBaseServ.iNikUserServ.Find(x => (x.UserName == request.NCode || x.Email == request.Email) && x.Id != request.Id);
                if (readyUser != null)
                {
                    AddError("کاربری با این کد ملی یا ایمیل قبلا ثبت شده است", "fa");
                    result = false;
                }
            }

            if (string.IsNullOrEmpty(request.NCode))
            {
                AddError("کد ملی باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.Mobile))
            {
                AddError("موبایل باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.Email))
            {
                AddError("آدرس ایمیل باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (request.Id == 0 && string.IsNullOrEmpty(request.Password))
            {
                AddError("رمز عبور باید مقدار داشته باشد", "fa");
                result = false;
            }
            else if (request.Id == 0 && request.Password.Length < 6)
            {
                AddError("رمز عبور باید بیشتر از 6 کاراکتر باشد", "fa");
                result = false;
            }

            if (!string.IsNullOrEmpty(request.ConfirmedPassword) && request.ConfirmedPassword != request.Password)
            {
                AddError("رمز عبور و تکرار آن باید یکسان باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.Mobile))
            {
                AddError("شماره موبایل باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.Firstname))
            {
                AddError("نام باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (request.Gender == 0)
            {
                AddError("جنسیت باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.Lastname))
            {
                AddError("نام خانوادگی باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.ZipCode))
            {
                AddError("کد پستی باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.BankName))
            {
                AddError("نام بانک باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.BranchName))
            {
                AddError("نام شعبه بانک باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.BranchCode))
            {
                AddError("کد شعبه باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.PAN))
            {
                AddError("شماره حساب باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.IBAN))
            {
                AddError("شماره شبا باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.BirthDate))
            {
                AddError("تاریخ تولد باید مقدار داشته باشد", "fa");
                result = false;
            }

            return result;
        }

        private void DropDownBinder(int provinceId, int genderId)
        {
            var provinces = ISystemBaseServ.iProvinceServ.GetAll(x => true);
            ViewBag.Provinces = new SelectList(provinces, "Id", "Title", provinceId);

            List<ListItemModel> genders = new List<ListItemModel>();
            genders.Add(new ListItemModel
            {
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
