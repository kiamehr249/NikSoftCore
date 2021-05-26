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
    public class SettingManage : NikController
    {
        public IBourseService iBourseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;

        public SettingManage(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
            this.userManager = userManager;
        }

        public IActionResult Index(SettingSearch request)
        {
            ViewBag.PageTitle = "تنظیمات سیستم";

            var query = iBourseServ.iSettingServ.ExpressionMaker();
            query.Add(x => true);
            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            if (request.ParentId > 0)
            {
                query.Add(x => x.ParentId == request.ParentId);
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iSettingServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;
            ComboBinder(request.ParentId);
            ViewBag.Contents = iBourseServ.iSettingServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View(request);
        }

        [HttpGet]
        public IActionResult Create(int Id)
        {
            ViewBag.PageTitle = "ایجاد تنظیم جدید";

            var request = new SettingRequest();
            if (Id > 0)
            {
                var item = iBourseServ.iSettingServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.FullText = item.FullText;
                request.KeyName = item.KeyName;
                request.MaxVal = item.MaxVal;
                request.MinVal = item.MinVal;
                request.ReferenceCode = item.ReferenceCode;
                request.Message = item.Message;
                request.ParentId = item.ParentId == null ? 0 : item.ParentId.Value;
            }
            ComboBinder(request.ParentId, request.Id);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SettingRequest request)
        {
            ViewBag.Messages = Messages;
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (!ValidForm(request))
            {
                ComboBinder(request.ParentId, request.Id);
                return View(request);
            }

            Setting item;
            if (request.Id > 0)
            {
                item = iBourseServ.iSettingServ.Find(x => x.Id == request.Id);
            }
            else
            {
                item = new Setting();
            }

            item.Title = request.Title;
            item.FullText = request.FullText;
            item.KeyName = request.KeyName;
            item.MaxVal = request.MaxVal;
            item.MinVal = request.MinVal;
            item.ReferenceCode = request.ReferenceCode;
            item.Message = request.Message;
            item.ParentId = request.ParentId;

            if (request.Id == 0)
            {
                iBourseServ.iSettingServ.Add(item);
            }

            await iBourseServ.iSettingServ.SaveChangesAsync();
            return Redirect("/Panel/SettingManage");

        }

        public async Task<IActionResult> Remove(int Id)
        {
            var item = await iBourseServ.iSettingServ.FindAsync(x => x.Id == Id);
            iBourseServ.iSettingServ.Remove(item);
            await iBourseServ.iSettingServ.SaveChangesAsync();
            return Redirect("/Panel/SettingManage");
        }

        private bool ValidForm(SettingRequest request)
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

        private void ComboBinder(int parentId, int itemId = 0)
        {
            var parents = iBourseServ.iSettingServ.GetAll(x => x.Id != itemId, y => new ListItemModel { Id = y.Id, Title = y.Title }).ToList();
            parents.Insert(0, new ListItemModel { 
                Id = 0,
                Title = "انتخاب کنید"
            });
            
            ViewBag.Parents = new SelectList(parents, "Id", "Title", parentId);
        }

    }
}
