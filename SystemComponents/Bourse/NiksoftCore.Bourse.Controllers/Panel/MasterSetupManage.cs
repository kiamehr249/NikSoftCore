using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    [Authorize(Roles = "NikAdmin,Admin")]
    public class MasterSetupManage : NikController
    {
        public IBourseService iBourseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;

        public MasterSetupManage(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
            this.userManager = userManager;
        }

        public IActionResult Index(MasterSearch request)
        {
            ViewBag.PageTitle = "سرپرست های شعب";

            var query = iBourseServ.iBranchMasterServ.ExpressionMaker();
            query.Add(x => true);
            bool isSearch = false;

            if (!string.IsNullOrEmpty(request.Firstname))
            {
                var fuIds = ISystemBaseServ.iUserProfileServ.GetAll(x => x.Firstname.Contains(request.Firstname), y => new { y.UserId }, 0, 20).Select(x => x.UserId).ToList();
                query.Add(x => fuIds.Contains(x.UserId));
                isSearch = true;
            }

            if (!string.IsNullOrEmpty(request.Lastname))
            {
                var fuIds = ISystemBaseServ.iUserProfileServ.GetAll(x => x.Lastname.Contains(request.Lastname), y => new { y.UserId }, 0, 20).Select(x => x.UserId).ToList();
                query.Add(x => fuIds.Contains(x.UserId));
                isSearch = true;
            }

            if (request.BranchId > 0)
            {
                query.Add(x => x.BranchId == request.BranchId);
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iBranchMasterServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iBourseServ.iBranchMasterServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            BranchBinder(request.BranchId);
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int Id)
        {
            ViewBag.PageTitle = "انتصاب سرپرست";

            var request = new BranchMasterRequest();
            if (Id > 0)
            {
                var item = await iBourseServ.iBranchMasterServ.FindAsync(x => x.Id == Id);
                request.Id = item.Id;
                request.UserId = item.UserId;
                request.BranchId = item.BranchId;
            }

            BranchBinder(request.BranchId);
            UserBinder(request.UserId);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BranchMasterRequest request)
        {
            ViewBag.PageTitle = "نصب سرپرست جدید";
            ViewBag.Messages = Messages;
            var user = await userManager.GetUserAsync(HttpContext.User);
            var masterUser = await userManager.FindByIdAsync(request.UserId.ToString());
            if (!ValidForm(request))
            {
                return View(request);
            }

            var readyItem = await iBourseServ.iBranchMasterServ.FindAsync(x => x.BranchId == request.BranchId);
            if (readyItem != null)
            {
                BranchBinder(request.BranchId);
                UserBinder(request.UserId);
                AddError("شعبه مورد نظر دارای سرپرست می باشد.", "fa");
                ViewBag.Messages = Messages;
                return View(request);
            }

            BranchMaster item;
            if (request.Id > 0)
            {
                item = iBourseServ.iBranchMasterServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }
            else
            {
                item = new BranchMaster();
                item.CreateDate = DateTime.Now;
                item.CreatedBy = user.Id;
            }

            item.UserId = request.UserId;
            item.BranchId = request.BranchId;

            if (request.Id == 0)
            {
                iBourseServ.iBranchMasterServ.Add(item);
            }

            await iBourseServ.iBranchMasterServ.SaveChangesAsync();
            var hasRole = await userManager.IsInRoleAsync(masterUser, "Master");
            if (!hasRole)
            {
                await userManager.AddToRoleAsync(masterUser, "Master");
            }

            return Redirect("/Panel/MasterSetupManage");

        }

        public async Task<IActionResult> Remove(int Id)
        {
            var item = await iBourseServ.iBranchMasterServ.FindAsync(x => x.Id == Id);

            var masterUser = await userManager.FindByIdAsync(item.UserId.ToString());
            await userManager.RemoveFromRoleAsync(masterUser, "Master");

            iBourseServ.iBranchMasterServ.Remove(item);
            await iBourseServ.iBranchMasterServ.SaveChangesAsync();
            return Redirect("/Panel/MasterSetupManage");
        }

        private bool ValidForm(BranchMasterRequest request)
        {
            if (request.UserId == 0)
            {
                AddError("شناسه کاربر باید مقدار داشته باشد", "fa");
            }

            if (request.BranchId == 0)
            {
                AddError("شعبه باید مقدار داشته باشد", "fa");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                return false;
            }

            return true;

        }

        private void BranchBinder(int branchId)
        {
            var branches = iBourseServ.iBranchServ.GetAll(x => true, y => new { y.Id, y.Title });
            branches.Insert(0, new { Id = 0, Title = "انتخاب کنید" });
            ViewBag.Branches = new SelectList(branches, "Id", "Title", branchId);
        }

        private void UserBinder(int userId)
        {
            var users = iBourseServ.iUserProfileServ.GetAll(x => true, y => new { y.Id, Title = y.Firstname + " " + y.Lastname });
            users.Insert(0, new { Id = 0, Title = "انتخاب کنید" });
            ViewBag.Users = new SelectList(users, "Id", "Title", userId);
        }

    }
}
