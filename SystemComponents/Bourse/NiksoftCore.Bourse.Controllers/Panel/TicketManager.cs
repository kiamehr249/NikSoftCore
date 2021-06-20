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
    public class TicketManager : NikController
    {
        public IBourseService iBourseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;

        public TicketManager(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
            this.userManager = userManager;
        }

        public IActionResult Index(TicketCategorySearch request)
        {
            ViewBag.PageTitle = "مدیریت دسته بندی تیکت";

            var query = iBourseServ.iTicketCategoryServ.ExpressionMaker();
            query.Add(x => true);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iTicketCategoryServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iBourseServ.iTicketCategoryServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View();
        }

        [HttpGet]
        public IActionResult CreateCategory(int Id)
        {
            ViewBag.PageTitle = "ایجاد دسته جدید";

            var request = new TicketCategoryRequest();
            if (Id > 0)
            {
                var item = iBourseServ.iTicketCategoryServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.KeyValue = item.KeyValue;
                request.Description = item.Description;
                request.Enabled = item.Enabled;
                request.OrderId = item.OrderId;

            }
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(TicketCategoryRequest request)
        {
            ViewBag.PageTitle = "ایجاد دسته جدید";
            ViewBag.Messages = Messages;
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (!ValidCategoryForm(request))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            TicketCategory item;
            if (request.Id > 0)
            {
                item = iBourseServ.iTicketCategoryServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }
            else
            {
                item = new TicketCategory();
                item.CreateDate = DateTime.Now;
                item.CreatedBy = user.Id;
            }

            item.Title = request.Title;
            item.KeyValue = request.KeyValue;
            item.Description = request.Description;

            if (request.Id == 0)
            {
                var maxOrder = iBourseServ.iTicketCategoryServ.Count(x => true);
                item.OrderId = maxOrder + 1;
                item.Enabled = true;
                iBourseServ.iTicketCategoryServ.Add(item);
            }

            await iBourseServ.iTicketCategoryServ.SaveChangesAsync();

            return Redirect("/Panel/TicketManager");

        }

        public async Task<IActionResult> Remove(int Id)
        {
            var item = await iBourseServ.iTicketCategoryServ.FindAsync(x => x.Id == Id);
            iBourseServ.iTicketCategoryServ.Remove(item);
            await iBourseServ.iTicketCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/TicketManager");
        }

        private bool ValidCategoryForm(TicketCategoryRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("عنوان باید مقدار داشته باشد", "fa");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                return false;
            }

            return true;

        }

        public async Task<IActionResult> OrderUpCat(int Id)
        {
            var theItem = iBourseServ.iTicketCategoryServ.Find(x => x.Id == Id);
            var itemsCount = iBourseServ.iTicketCategoryServ.Count(x => true);
            if (theItem.OrderId == itemsCount)
            {
                return Redirect("/Panel/TicketManager");
            }

            var upItem = iBourseServ.iTicketCategoryServ.Find(x => x.OrderId == (theItem.OrderId + 1));

            theItem.OrderId = theItem.OrderId + 1;
            upItem.OrderId = upItem.OrderId - 1;

            await iBourseServ.iTicketCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/TicketManager");
        }

        public async Task<IActionResult> OrderDownCat(int Id)
        {
            var theItem = iBourseServ.iTicketCategoryServ.Find(x => x.Id == Id);
            if (theItem.OrderId == 1)
            {
                return Redirect("/Panel/TicketManager");
            }

            var upItem = iBourseServ.iTicketCategoryServ.Find(x => x.OrderId == (theItem.OrderId - 1));

            theItem.OrderId = theItem.OrderId - 1;
            upItem.OrderId = upItem.OrderId + 1;

            await iBourseServ.iTicketCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/TicketManager");
        }





        private void ComboBinder(int areaId)
        {
            var areas = iBourseServ.iBranchAreaServ.GetAll(x => x.Enabled, y => new { y.Id, y.Title }).ToList();
            ViewBag.Areas = new SelectList(areas, "Id", "Title", areaId);
        }

    }
}
