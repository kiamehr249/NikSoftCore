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
    [Authorize(Roles = "NikAdmin,Admin")]
    public class FeeManage : NikController
    {
        public IBourseService iBourseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;

        public FeeManage(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
            this.userManager = userManager;
        }

        public IActionResult Index(FeeSearch request)
        {
            ViewBag.PageTitle = "مدیریت نرخ کارمزد";

            var query = iBourseServ.iFeeServ.ExpressionMaker();
            query.Add(x => true);
            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            if (request.FeeType > 0)
            {
                query.Add(x => x.FeeType == (FeeType)request.FeeType);
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iFeeServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iBourseServ.iFeeServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View();
        }

        [HttpGet]
        public IActionResult Create(int Id)
        {

            ViewBag.PageTitle = "ایجاد نرخ جدید";

            var request = new Fee();
            if (Id > 0)
            {
                request = iBourseServ.iFeeServ.Find(x => x.Id == Id);
            }
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Fee request)
        {
            ViewBag.Messages = Messages;
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (!ValidForm(request))
            {
                return View(request);
            }

            Fee item;
            if (request.Id > 0)
            {
                item = iBourseServ.iFeeServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }
            else
            {
                item = new Fee();
                item.CreateDate = DateTime.Now;
                item.CreatedBy = user.Id;
            }

            item.Title = request.Title;
            item.FeeType = request.FeeType;
            item.FromAmount = request.FromAmount;
            item.ToAmount = request.ToAmount;

            if (request.Id == 0)
            {
                iBourseServ.iFeeServ.Add(item);
            }

            await iBourseServ.iFeeServ.SaveChangesAsync();

            return Redirect("/Panel/FeeManage");

        }

        public async Task<IActionResult> Remove(int Id)
        {
            var item = await iBourseServ.iFeeServ.FindAsync(x => x.Id == Id);
            iBourseServ.iFeeServ.Remove(item);
            await iBourseServ.iFeeServ.SaveChangesAsync();
            return Redirect("/Panel/FeeManage");
        }

        private bool ValidForm(Fee request)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("عنوان نرخ باید مقدار داشته باشد", "fa");
            }

            if (request.FromAmount == 0)
            {
                AddError("از مبلغ باید مقدار داشته باشد", "fa");
            }

            if (request.ToAmount == 0)
            {
                AddError("تا مبلغ باید مقدار داشته باشد", "fa");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                return false;
            }

            return true;

        }

    }
}
