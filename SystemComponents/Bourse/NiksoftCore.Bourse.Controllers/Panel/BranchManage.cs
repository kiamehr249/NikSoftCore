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
    public class BranchManage : NikController
    {
        public IBourseService iBourseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;

        public BranchManage(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
            this.userManager = userManager;
        }

        public IActionResult Index(BranchSearch request)
        {
            ViewBag.PageTitle = "مدیریت شعب";

            var query = iBourseServ.iBranchServ.ExpressionMaker();
            query.Add(x => true);
            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            if (!string.IsNullOrEmpty(request.Code))
            {
                query.Add(x => x.Code.Contains(request.Code));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iBranchServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iBourseServ.iBranchServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View();
        }

        [HttpGet]
        public IActionResult Create(int Id)
        {

            ViewBag.PageTitle = "ایجاد شعبه جدید";

            var request = new Branch();
            if (Id > 0)
            {
                request = iBourseServ.iBranchServ.Find(x => x.Id == Id);
            }

            ComboBinder(request.AreaId);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Branch request)
        {
            ViewBag.Messages = Messages;
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (!ValidForm(request))
            {
                ViewBag.Messages = Messages;
                ComboBinder(request.AreaId);
                return View(request);
            }

            Branch item;
            if (request.Id > 0)
            {
                item = iBourseServ.iBranchServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }
            else
            {
                item = new Branch();
                item.CreateDate = DateTime.Now;
                item.CreatedBy = user.Id;
            }

            item.Title = request.Title;
            item.Code = request.Code;
            item.AreaId = request.AreaId;

            if (request.Id == 0)
            {
                iBourseServ.iBranchServ.Add(item);
            }

            await iBourseServ.iBranchServ.SaveChangesAsync();

            return Redirect("/Panel/BranchManage");

        }

        public async Task<IActionResult> Remove(int Id)
        {
            var item = await iBourseServ.iBranchServ.FindAsync(x => x.Id == Id);
            iBourseServ.iBranchServ.Remove(item);
            await iBourseServ.iBranchServ.SaveChangesAsync();
            return Redirect("/Panel/BranchManage");
        }

        private bool ValidForm(Branch request)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("نام شعبه باید مقدار داشته باشد", "fa");
            }

            if (string.IsNullOrEmpty(request.Code))
            {
                AddError("کد شعبه باید مقدار داشته باشد", "fa");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                return false;
            }

            return true;

        }

        private void ComboBinder(int areaId)
        {
            var areas = iBourseServ.iBranchAreaServ.GetAll(x => x.Enabled, y => new { y.Id, y.Title }).ToList();
            ViewBag.Areas = new SelectList(areas, "Id", "Title", areaId);
        }

    }
}
