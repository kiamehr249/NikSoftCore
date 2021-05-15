using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.Bourse.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System;
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

        public MarketerBoard(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index(MediaSearch request)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var marketerUser = await iBourseServ.iBranchMarketerServ.FindAsync(x => x.UserId == user.Id);
            if (marketerUser == null)
            {
                return Redirect("/Panel");
            }

            var userProfile = ISystemBaseServ.iUserProfileServ.Find(x => x.UserId == user.Id);

            ViewBag.PageTitle = "مشاوران های " + userProfile.Firstname + " " + userProfile.Lastname;
            
            var query = iBourseServ.iMediaServ.ExpressionMaker();
            query.Add(x => x.UserId == user.Id);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            if (request.CategoryId > 0)
            {
                query.Add(x => x.CategoryId == request.CategoryId);
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iMediaServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;
            ViewBag.Contents = iBourseServ.iMediaServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
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
        public async Task<IActionResult> AddConsultant(BourseUserRequest request)
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
                var readyUser = ISystemBaseServ.iNikUserServ.Find(x => x.PhoneNumber == request.Mobile || x.Email == request.Email);
                if (readyUser != null)
                {
                    AddError("کاربری با این شماره موبایل یا ایمیل قبلا ثبت شده است", "fa");
                    result = false;
                }
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
                request.Deadline = contract.Deadline;
                request.Status = contract.Status;
                request.ContractDate = contract.ContractDate.ToPersianDateTime().ToPersianDigitalDateString();
            }

            request.UserId = UserId;
            request.UserFullName = theProfile.Firstname + " " + theProfile.Lastname;
            request.BranchId = marketerUser.BranchId;

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
                return View(request);
            }

            Contract item = new Contract();
            if (request.Id > 0)
            {
                item = iBourseServ.iContractServ.Find(x => x.Id == request.Id);
            }


            item.ContractNumber = request.ContractNumber;
            item.ContractType = ContractType.Marketer;
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

    }
}
