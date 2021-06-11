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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.Bourse.Controllers.Panel
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin,Admin")]
    public class BranchAreaManage : NikController
    {
        public IBourseService iBourseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;

        public BranchAreaManage(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
            this.userManager = userManager;
        }

        public IActionResult Index(AreaSearch request)
        {
            ViewBag.PageTitle = "مدیریت مناطق";

            var query = iBourseServ.iBranchAreaServ.ExpressionMaker();
            query.Add(x => true);
            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iBranchAreaServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iBourseServ.iBranchAreaServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View();
        }

        [HttpGet]
        public IActionResult Create(int Id)
        {
            ViewBag.PageTitle = "ایجاد منطقه";
            var request = new AreaRequest();
            if (Id > 0)
            {
                var item = iBourseServ.iBranchAreaServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.Description = item.Description;
                request.Enabled = item.Enabled;
            }
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AreaRequest request)
        {
            ViewBag.Messages = Messages;
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (!ValidForm(request))
            {
                return View(request);
            }

            BranchArea item = new BranchArea();
            if (request.Id > 0)
            {
                item = iBourseServ.iBranchAreaServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }

            item.Title = request.Title;
            item.Description = request.Description;

            if (request.Id == 0)
            {
                item.Enabled = true;
                item.CreateDate = DateTime.Now;
                item.CreatedBy = user.Id;
                iBourseServ.iBranchAreaServ.Add(item);
            }
            else
            {
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }

            await iBourseServ.iBranchAreaServ.SaveChangesAsync();
            return Redirect("/Panel/BranchAreaManage");

        }

        public async Task<IActionResult> Remove(int Id)
        {
            var item = await iBourseServ.iBranchAreaServ.FindAsync(x => x.Id == Id);
            iBourseServ.iBranchAreaServ.Remove(item);
            await iBourseServ.iBranchAreaServ.SaveChangesAsync();
            return Redirect("/Panel/BranchAreaManage");
        }

        public async Task<IActionResult> Enable(int Id)
        {
            var item = await iBourseServ.iBranchAreaServ.FindAsync(x => x.Id == Id);
            item.Enabled = !item.Enabled;
            await iBourseServ.iBranchAreaServ.SaveChangesAsync();
            return Redirect("/Panel/BranchAreaManage");
        }

        private bool ValidForm(AreaRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("عنوان نرخ باید مقدار داشته باشد", "fa");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                return false;
            }

            return true;

        }

    }
}
