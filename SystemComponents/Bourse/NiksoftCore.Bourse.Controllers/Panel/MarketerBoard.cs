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
using System.Threading.Tasks;

namespace NiksoftCore.Bourse.Controllers.Panel
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin,Admin,Marketer")]
    public class MarketerBoard : NikController
    {
        public IBourseService iBourseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;
        private readonly IWebHostEnvironment hosting;

        public MarketerBoard(IConfiguration Configuration,
            IWebHostEnvironment hosting,
            UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
            this.userManager = userManager;
            this.hosting = hosting;
        }

        public async Task<IActionResult> Index(ConsultantSearch request)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var marketerUser = await iBourseServ.iBranchMarketerServ.FindAsync(x => x.UserId == user.Id);
            if (marketerUser == null)
            {
                return Redirect("/Panel");
            }

            var userProfile = ISystemBaseServ.iUserProfileServ.Find(x => x.UserId == user.Id);

            ViewBag.PageTitle = "مشاورهای " + userProfile.Firstname + " " + userProfile.Lastname;
            
            var query = iBourseServ.iBranchConsultantServ.ExpressionMaker();
            query.Add(x => x.MarketerId == user.Id);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Firstname))
            {
                var userIds = ISystemBaseServ.iUserProfileServ.GetAll(x => x.Firstname.Contains(request.Firstname), y => new { y.UserId }, 0, 20).Select(x => x.UserId).ToList();
                query.Add(x => userIds.Contains(x.UserId));
                isSearch = true;
            }

            if (!string.IsNullOrEmpty(request.Lastname))
            {
                var userIds = ISystemBaseServ.iUserProfileServ.GetAll(x => x.Lastname.Contains(request.Lastname), y => new { y.UserId }, 0, 20).Select(x => x.UserId).ToList();
                query.Add(x => userIds.Contains(x.UserId));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iBranchConsultantServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;
            ViewBag.Contents = iBourseServ.iBranchConsultantServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> AddConsultant(int UserId)
        {
            ViewBag.PageTitle = "ایجاد بازاریاب";
            var user = await userManager.GetUserAsync(HttpContext.User);
            var marketerUser = await iBourseServ.iBranchMarketerServ.FindAsync(x => x.UserId == user.Id);

            var request = new BourseUserRequest();
            request.BranchId = marketerUser.BranchId;
            if (UserId > 0)
            {
                var theUser = ISystemBaseServ.iNikUserServ.Find(x => x.Id == UserId);
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
                    request.BirthDate = theProfile.BirthDate.Value.ToPersianDateTime().ToPersianDigitalDateString();
                    request.Avatar = theProfile.Avatar;
                    request.IdCardImage = theProfile.IdCardImage;
                    request.NCardImage = theProfile.NCardImage;
                    request.Status = theProfile.Status;
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
        public async Task<IActionResult> AddConsultant(BourseUserRequest request)
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

                iBourseServ.iBranchUserServ.Add(new BranchUser
                {
                    UserId = item.Id,
                    BranchId = request.BranchId,
                    UserType = BranchUserType.Consultant
                });
                await iBourseServ.iBranchUserServ.SaveChangesAsync();

                await userManager.AddToRoleAsync(item, "Consultant");
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
            profile.Gender = request.Gender == 0 ? null : request.Gender;

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

            if (request.Id == 0)
            {
                iBourseServ.iBranchConsultantServ.Add(new BranchConsultant
                {
                    UserId = item.Id,
                    BranchId = request.BranchId,
                    MarketerId = user.Id,
                    CreateDate = DateTime.Now,
                    CreatedBy = user.Id
                });

                await iBourseServ.iBranchConsultantServ.SaveChangesAsync();
            }

            return Redirect("/Panel/MarketerBoard");

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

            if (request.Gender == 0)
            {
                AddError("جنسیت باید مقدار داشته باشد", "fa");
                result = false;
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


        public async Task<IActionResult> ConsultantContracts(MarketerContractSearch request)
        {
            var theConsult = await ISystemBaseServ.iUserProfileServ.FindAsync(x => x.UserId == request.UserId);
            ViewBag.PageTitle = "قرارداد های " + theConsult.Firstname + " " + theConsult.Lastname;

            var user = await userManager.GetUserAsync(HttpContext.User);
            var marketerUser = await iBourseServ.iBranchMarketerServ.FindAsync(x => x.UserId == user.Id);

            var isOwnUser = await iBourseServ.iBranchConsultantServ.FindAsync(x => x.UserId == request.UserId && x.MarketerId == user.Id);
            if (isOwnUser == null)
            {
                return Redirect("/Panel/MarketerBoard");
            }

            var query = iBourseServ.iContractServ.ExpressionMaker();
            query.Add(x => x.BranchId == marketerUser.BranchId && x.UserId == request.UserId);

            bool isSearch = false;
            ViewBag.Search = isSearch;

            var total = iBourseServ.iContractServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;
            ViewBag.BranchId = request.BranchId;
            ViewBag.Contents = iBourseServ.iContractServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();

            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> AddContract(int Id, int UserId)
        {
            ViewBag.PageTitle = "ایجاد قرارداد";
            var user = await userManager.GetUserAsync(HttpContext.User);
            var marketerUser = await iBourseServ.iBranchMarketerServ.FindAsync(x => x.UserId == user.Id);

            var theProfile = await ISystemBaseServ.iUserProfileServ.FindAsync(x => x.UserId == UserId);
            var request = new ContractRequest();

            if (Id > 0)
            {
                var contract = iBourseServ.iContractServ.Find(x => x.Id == Id);
                request.Id = contract.Id;
                request.ContractNumber = contract.ContractNumber;
                request.ContractType = contract.ContractType;
                request.UserCode = contract.UserCode;
                request.StartDate = contract.StartDate.ToPersianDateTime().ToPersianDigitalDateString();
                request.EndDate = contract.EndDate.ToPersianDateTime().ToPersianDigitalDateString();
                request.FeeId = contract.FeeId;
                request.FeeType = (int)contract.Fee.FeeType;
                request.Deadline = contract.Deadline;
                request.Status = contract.Status;
                request.ContractDate = contract.ContractDate.ToPersianDateTime().ToPersianDigitalDateString();
            }

            request.UserId = UserId;
            request.UserFullName = theProfile.Firstname + " " + theProfile.Lastname;
            request.BranchId = marketerUser.BranchId;
            FeeTypeBinder(request.FeeType);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> AddContract(ContractRequest request)
        {
            ViewBag.Messages = Messages;
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (!ValidContractForm(request))
            {
                ViewBag.Messages = Messages;
                FeeTypeBinder(request.FeeType);
                return View(request);
            }

            Contract item = new Contract();
            if (request.Id > 0)
            {
                item = iBourseServ.iContractServ.Find(x => x.Id == request.Id);
            }


            item.ContractNumber = request.ContractNumber;
            item.ContractType = ContractType.Consultant;
            item.UserId = request.UserId;
            item.UserCode = request.UserCode;
            item.UserFullName = request.UserFullName;
            item.StartDate = PersianDateTime.Parse(request.StartDate).ToDateTime();
            item.EndDate = PersianDateTime.Parse(request.EndDate).ToDateTime();
            item.FeeId = request.FeeId;
            item.Deadline = request.Deadline;
            item.Status = ContractStatus.InProccess;
            item.ContractDate = PersianDateTime.Parse(request.ContractDate).ToDateTime();
            item.BranchId = request.BranchId;


            if (request.Id == 0)
            {
                item.CreateDate = DateTime.Now;
                item.CreatedBy = user.Id;
                iBourseServ.iContractServ.Add(item);
            }

            await iBourseServ.iContractServ.SaveChangesAsync();

            return Redirect("/Panel/MarketerBoard/ConsultantContracts/?UserId=" + request.UserId);

        }

        public bool ValidContractForm(ContractRequest request)
        {
            bool result = true;
            if (string.IsNullOrEmpty(request.ContractNumber))
            {
                AddError("شماره قرارداد باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.UserCode))
            {
                AddError("کد طرف قرارداد باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.StartDate))
            {
                AddError("تاریخ شروع قرارداد باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.EndDate))
            {
                AddError("تاریخ پایان قرارداد باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.EndDate))
            {
                AddError("تاریخ قرارداد باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (request.FeeId == 0)
            {
                AddError("کارمزد باید مقدار داشته باشد", "fa");
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

        private void FeeTypeBinder(int feeType)
        {
            List<ListItemModel> feeTypes = new List<ListItemModel>();
            feeTypes.Add(new ListItemModel
            {
                Id = 1,
                Title = "ثابت"
            });
            feeTypes.Add(new ListItemModel
            {
                Id = 2,
                Title = "پلکانی"
            });
            ViewBag.FeeTypes = new SelectList(feeTypes, "Id", "Title", feeType);
        }
    }
}
