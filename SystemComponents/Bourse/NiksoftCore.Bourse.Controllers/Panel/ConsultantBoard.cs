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
    [Authorize(Roles = "NikAdmin,Admin,Consultant")]
    public class ConsultantBoard : NikController
    {
        public IBourseService iBourseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;

        public ConsultantBoard(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index(MediaSearch request)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var ConsUser = await iBourseServ.iBranchConsultantServ.FindAsync(x => x.UserId == user.Id);
            if (ConsUser == null)
            {
                return Redirect("/Panel");
            }

            var userProfile = ISystemBaseServ.iUserProfileServ.Find(x => x.UserId == user.Id);

            ViewBag.PageTitle = "رسانه های من";

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
            ComboBinder(request.CategoryId);
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> AddMedia(int Id)
        {
            ViewBag.PageTitle = "ایجاد رسانه";
            var user = await userManager.GetUserAsync(HttpContext.User);

            var theProfile = await ISystemBaseServ.iUserProfileServ.FindAsync(x => x.UserId == user.Id);

            var request = new MediaRequest();

            if (Id > 0)
            {
                var item = iBourseServ.iMediaServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.Subject = item.Subject;
                request.BaseLink = item.BaseLink;
                request.FullLink = item.FullLink;
                //request.GeneratedLink = item.GeneratedLink;
                //request.ClickCount = item.ClickCount;
                //request.Status = item.Status;
                request.CategoryId = item.CategoryId;
            }

            request.UserId = user.Id;
            ComboBinder(request.CategoryId);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> AddMedia(MediaRequest request)
        {
            ViewBag.Messages = Messages;
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (!ValidMediaForm(request))
            {
                ViewBag.Messages = Messages;
                ComboBinder(request.CategoryId);
                return View(request);
            }

            Media item = new Media();
            if (request.Id > 0)
            {
                item = iBourseServ.iMediaServ.Find(x => x.Id == request.Id);
            }


            item.Title = request.Title;
            item.Subject = request.Subject;
            item.BaseLink = request.BaseLink;
            item.FullLink = request.FullLink;
            item.CategoryId = request.CategoryId;


            if (request.Id == 0)
            {
                item.Status = MediaStatus.InProccess;
                item.UserId = user.Id;
                item.CreateDate = DateTime.Now;
                item.CreatedBy = user.Id;
                iBourseServ.iMediaServ.Add(item);
            }

            await iBourseServ.iMediaServ.SaveChangesAsync();

            if (request.Id == 0)
            {
                var theNewItem = await iBourseServ.iMediaServ.FindAsync(x => x.Id == item.Id);
                item.GeneratedLink = "/LandingPage/Media/" + theNewItem.Id;
                await iBourseServ.iMediaServ.SaveChangesAsync();
            }


            return Redirect("/Panel/ConsultantBoard");

        }

        public async Task<IActionResult> RemoveMedia(int Id)
        {
            var item = iBourseServ.iMediaServ.Find(x => x.Id == Id);
            iBourseServ.iMediaServ.Remove(item);
            await iBourseServ.iMediaServ.SaveChangesAsync();
            return Redirect("/Panel/ConsultantBoard");
        }

        public bool ValidMediaForm(MediaRequest request)
        {
            bool result = true;
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("عنوان باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.BaseLink))
            {
                AddError("آدرس اصلی باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.FullLink))
            {
                AddError("آدرس کامل باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (request.CategoryId == 0)
            {
                AddError("نوع رسانه باید مقدار داشته باشد", "fa");
                result = false;
            }

            return result;
        }

        private void ComboBinder(int CategoryId)
        {
            var categories = iBourseServ.iMediaCategoryServ.GetAll(x => true, y => new { y.Id, y.Title });
            ViewBag.Categories = new SelectList(categories, "Id", "Title", CategoryId);
        }

    }
}
