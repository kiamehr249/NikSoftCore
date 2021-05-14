using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize(Roles = "NikAdmin")]
    public class BUserManager : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly RoleManager<DataModel.Role> roleManager;
        public IBourseService iBourseServ { get; set; }

        public BUserManager(IConfiguration Configuration,
            UserManager<DataModel.User> userManager,
            RoleManager<DataModel.Role> roleManager) : base(Configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
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
            var pager = new Pagination(total, 20, request.part);
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
                request.Mobile = theUser.UserName;
                request.Email = theUser.Email;
                request.Mobile = theUser.PhoneNumber;

                var theProfile = ISystemBaseServ.iUserProfileServ.Find(x => x.UserId == theUser.Id);
                if (theProfile != null)
                {
                    request.ProfileId = theProfile.Id;
                    request.Firstname = theProfile.Firstname;
                    request.Lastname = theProfile.Lastname;
                    request.Mobile = theProfile.Mobile;
                    request.Tel = theProfile.Tel;
                    request.Address = theProfile.Address;
                    request.ZipCode = theProfile.ZipCode;
                    request.BirthDate = theProfile.BirthDate.Value.ToPersianDateTime().ToPersianDigitalDateString();
                    request.Avatar = theProfile.Avatar;
                    request.IdCardImage = theProfile.IdCardImage;
                    request.NCardImage = theProfile.NCardImage;
                    request.Status = theProfile.Status;
                    request.ProvinceId = theProfile.ProvinceId ?? 0;
                    request.CityId = theProfile.CityId ?? 0;
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

            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BourseUserRequest request)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (!ValidUserForm(request))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            DataModel.User item = new DataModel.User();
            if (request.Id > 0)
            {
                item = await userManager.FindByIdAsync(request.Id.ToString());
            }


            item.UserName = request.Mobile;
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
                await userManager.UpdateAsync(item);
            }

            SystemBase.Service.UserProfile profile = new SystemBase.Service.UserProfile();
            if (request.ProfileId > 0)
            {
                profile = ISystemBaseServ.iUserProfileServ.Find(x => x.UserId == item.Id);
            }

            profile.Firstname = request.Firstname;
            profile.Lastname = request.Lastname;
            profile.CompanyName = request.CompanyName;
            profile.Mobile = request.Mobile;
            profile.Tel = request.Tel;
            profile.Address = request.Address;
            profile.ZipCode = request.ZipCode;
            profile.BirthDate = PersianDateTime.Parse(request.BirthDate).ToDateTime();
            //profile.Avatar = request.Avatar;
            //profile.IdCardImage = request.IdCardImage;
            //profile.NCardImage = request.NCardImage;
            profile.ProvinceId = request.ProvinceId == 0 ? null : request.ProvinceId;
            profile.CityId = request.CityId == 0 ? null : request.CityId;

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
            var readyUser = ISystemBaseServ.iNikUserServ.Find(x => x.PhoneNumber == request.Mobile);
            if (readyUser != null)
            {
                AddError("کاربری با این شماره موبایل قبلا ثبت شده است", "fa");
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

            if (request.Id == 0 && (string.IsNullOrEmpty(request.ConfirmedPassword) || request.ConfirmedPassword != request.Password))
            {
                AddError("رمز عبور باید مقدار داشته باشد", "fa");
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


            return result;
        }


    }
}
