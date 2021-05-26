using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.Bourse.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.Bourse.Controllers.Panel
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin,Admin")]
    public class AdminBoard : NikController
    {
        public IBourseService iBourseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;

        public AdminBoard(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index(AdminContractSearch request)
        {
            ViewBag.PageTitle = "مدیریت قراردادها";
            var user = await userManager.GetUserAsync(HttpContext.User);
            var query = iBourseServ.iContractServ.ExpressionMaker();
            query.Add(x => true);
            bool isSearch = false;

            if (!string.IsNullOrEmpty(request.ContractNumber))
            {
                query.Add(x => x.ContractNumber.Contains(request.ContractNumber));
                isSearch = true;
            }

            if (!string.IsNullOrEmpty(request.UserFullName))
            {
                query.Add(x => x.UserFullName.Contains(request.UserFullName));
                isSearch = true;
            }

            if (request.ContractType > 0)
            {
                query.Add(x => x.ContractType == (ContractType)request.ContractType);
                isSearch = true;
            }

            if (request.Status > 0)
            {
                query.Add(x => x.Status == (ContractStatus)request.Status);
                isSearch = true;
            }

            if (request.BranchId > 0)
            {
                query.Add(x => x.BranchId == request.BranchId);
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iContractServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;
            ViewBag.Contents = iBourseServ.iContractServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();

            BranchBinder(request.BranchId);
            return View(request);
        }

        public async Task<IActionResult> Accept(int Id)
        {
            var item = await iBourseServ.iContractServ.FindAsync(x => x.Id == Id);
            item.Status = ContractStatus.Accept;
            await iBourseServ.iContractServ.SaveChangesAsync();
            return Redirect("/Panel/AdminBoard");
        }

        public async Task<IActionResult> Ignore(int Id)
        {
            var item = await iBourseServ.iContractServ.FindAsync(x => x.Id == Id);
            item.Status = ContractStatus.Ignore;
            await iBourseServ.iContractServ.SaveChangesAsync();
            return Redirect("/Panel/AdminBoard");
        }

        public async Task<IActionResult> InProgress(int Id)
        {
            var item = await iBourseServ.iContractServ.FindAsync(x => x.Id == Id);
            item.Status = ContractStatus.InProccess;
            await iBourseServ.iContractServ.SaveChangesAsync();
            return Redirect("/Panel/AdminBoard");
        }


        public async Task<IActionResult> MediaManage(AdminMediaSearch request)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            ViewBag.PageTitle = "مدیریت رسانه ها";

            var query = iBourseServ.iMediaServ.ExpressionMaker();
            query.Add(x => true);
            bool isSearch = false;

            if (request.Status > 0)
            {
                query.Add(x => x.Status == (MediaStatus)request.Status);
                isSearch = true;
            }

            if (request.BranchId > 0)
            {
                query.Add(x => x.BranchId == request.BranchId);
                isSearch = true;
            }

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
            CategoryBinder(request.CategoryId);
            BranchBinder(request.BranchId);
            return View(request);
        }

        public async Task<IActionResult> OwnerState(int Id)
        {
            var item = await iBourseServ.iMediaServ.FindAsync(x => x.Id == Id);
            item.Ownership = !item.Ownership;
            await iBourseServ.iMediaServ.SaveChangesAsync();
            return Redirect("/Panel/AdminBoard/MediaManage");
        }


        private void CategoryBinder(int CategoryId)
        {
            var categories = iBourseServ.iMediaCategoryServ.GetAll(x => true, y => new { y.Id, y.Title });
            ViewBag.Categories = new SelectList(categories, "Id", "Title", CategoryId);
        }

        private void BranchBinder(int branchId)
        {
            var branches = iBourseServ.iBranchServ.GetAll(x => true, y => new { y.Id, y.Title });
            ViewBag.Branches = new SelectList(branches, "Id", "Title", branchId);
        }

        public async Task<IActionResult> AcceptMedia(int Id)
        {
            var item = await iBourseServ.iMediaServ.FindAsync(x => x.Id == Id);
            item.Status = MediaStatus.Accept;
            await iBourseServ.iMediaServ.SaveChangesAsync();
            return Redirect("/Panel/AdminBoard/MediaManage");
        }

        public async Task<IActionResult> IgnoreMedia(int Id)
        {
            var item = await iBourseServ.iMediaServ.FindAsync(x => x.Id == Id);
            item.Status = MediaStatus.Ignore;
            await iBourseServ.iMediaServ.SaveChangesAsync();
            return Redirect("/Panel/AdminBoard/MediaManage");
        }

        public async Task<IActionResult> InProgressMedia(int Id)
        {
            var item = await iBourseServ.iMediaServ.FindAsync(x => x.Id == Id);
            item.Status = MediaStatus.InProccess;
            await iBourseServ.iMediaServ.SaveChangesAsync();
            return Redirect("/Panel/AdminBoard/MediaManage");
        }


        public async Task<IActionResult> Marketers(MarketerSearch request)
        {
            ViewBag.PageTitle = "بازاریاب ها";
            var user = await userManager.GetUserAsync(HttpContext.User);

            var query = iBourseServ.iBranchMarketerServ.ExpressionMaker();
            query.Add(x => true);
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

            if (request.BranchId > 0)
            {
                query.Add(x => x.BranchId == request.BranchId);
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iBranchMarketerServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;
            ViewBag.Contents = iBourseServ.iBranchMarketerServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();

            BranchBinder(request.BranchId);
            return View(request);
        }

        public async Task<IActionResult> Consultants(ConsultantSearch request)
        {
            ViewBag.PageTitle = "مشاورین";
            var user = await userManager.GetUserAsync(HttpContext.User);

            var query = iBourseServ.iBranchConsultantServ.ExpressionMaker();
            query.Add(x => true);
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

            if (request.BranchId > 0)
            {
                query.Add(x => x.BranchId == request.BranchId);
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iBranchConsultantServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;
            ViewBag.Contents = iBourseServ.iBranchConsultantServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();

            BranchBinder(request.BranchId);
            return View(request);
        }

        public string GetBranchMaster(int branchId)
        {
            var bMaster = iBourseServ.iBranchMasterServ.Find(x => x.BranchId == branchId);
            var profile = iBourseServ.iUserProfileServ.Find(x => x.UserId == bMaster.UserId);
            return profile.Firstname + " " + profile.Lastname;
        }


        [HttpGet]
        public IActionResult EditLawSetting()
        {
            ViewBag.PageTitle = "قوانین و مقررات";

            var request = new SettingRequest();
            var item = iBourseServ.iSettingServ.Find(x => x.KeyName == "marketerlaw");
            if (item == null)
            {
                return Redirect("/Panel");
            }

            request.Id = item.Id;
            request.Title = item.Title;
            request.FullText = item.FullText;
            request.KeyName = item.KeyName;
            request.MaxVal = item.MaxVal;
            request.MinVal = item.MinVal;
            request.ReferenceCode = item.ReferenceCode;
            request.Message = item.Message;
            request.ParentId = item.ParentId == null ? 0 : item.ParentId.Value;
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> EditLawSetting(SettingRequest request)
        {
            ViewBag.Messages = Messages;
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (!ValidLawForm(request))
            {
                return View(request);
            }

            var item = iBourseServ.iSettingServ.Find(x => x.Id == request.Id);

            item.Title = request.Title;
            item.FullText = request.FullText;

            await iBourseServ.iSettingServ.SaveChangesAsync();
            AddSuccess("ذخیره تغییرات با موفقیت انجام شد");
            return View(request);
        }

        private bool ValidLawForm(SettingRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("عنوان باید مقدار داشته باشد", "fa");
            }

            if (string.IsNullOrEmpty(request.FullText))
            {
                AddError("متن باید مقدار داشته باشد", "fa");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                return false;
            }

            return true;

        }

    }
}
