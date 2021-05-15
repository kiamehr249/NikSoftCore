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
    [Authorize(Roles = "NikAdmin,Admin")]
    public class MediaCategoryManage : NikController
    {
        public IBourseService iBourseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;

        public MediaCategoryManage(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
            this.userManager = userManager;
        }

        public IActionResult Index(MediaCategorySearch request)
        {
            ViewBag.PageTitle = "مدیریت دسته بندی های رسانه";

            var query = iBourseServ.iMediaCategoryServ.ExpressionMaker();
            query.Add(x => true);
            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iMediaCategoryServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iBourseServ.iMediaCategoryServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View();
        }

        [HttpGet]
        public IActionResult Create(int Id)
        {

            ViewBag.PageTitle = "ایجاد نرخ جدید";

            var request = new MediaCategory();
            if (Id > 0)
            {
                request = iBourseServ.iMediaCategoryServ.Find(x => x.Id == Id);
            }
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MediaCategory request)
        {
            ViewBag.Messages = Messages;
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (!ValidForm(request))
            {
                return View(request);
            }

            MediaCategory item = new MediaCategory();
            if (request.Id > 0)
            {
                item = iBourseServ.iMediaCategoryServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }
            else
            {
                item.CreateDate = DateTime.Now;
                item.CreatedBy = user.Id;
            }

            item.Title = request.Title;
            item.Description = request.Description;
            item.Enabled = true;

            if (request.Id == 0)
            {
                iBourseServ.iMediaCategoryServ.Add(item);
            }

            await iBourseServ.iMediaCategoryServ.SaveChangesAsync();

            return Redirect("/Panel/MediaCategoryManage");

        }

        public async Task<IActionResult> Enable(int Id)
        {
            var item = await iBourseServ.iMediaCategoryServ.FindAsync(x => x.Id == Id);
            item.Enabled = !item.Enabled;
            await iBourseServ.iMediaCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/MediaCategoryManage");
        }

        public async Task<IActionResult> Remove(int Id)
        {
            var item = await iBourseServ.iMediaCategoryServ.FindAsync(x => x.Id == Id);
            iBourseServ.iMediaCategoryServ.Remove(item);
            try
            {
                await iBourseServ.iMediaCategoryServ.SaveChangesAsync();
            }
            catch 
            {
                AddError("به دلیل داشتن محتوا در این دسته بندی امکان حذف وجود ندارد", "fa");
            }
            
            return Redirect("/Panel/MediaCategoryManage");
        }

        private bool ValidForm(MediaCategory request)
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