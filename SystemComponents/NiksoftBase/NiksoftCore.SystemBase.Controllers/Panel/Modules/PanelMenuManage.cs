using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.SystemBase.Service;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.SystemBase.Controllers.Panel.Modules
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin")]
    public class PanelMenuManage : NikController
    {

        public PanelMenuManage(IConfiguration Configuration) : base(Configuration)
        {

        }

        public IActionResult Index([FromQuery] string lang, int part)
        {
            var total = ISystemBaseServ.iPanelMenuService.Count(x => x.ParentId == null);
            var pager = new Pagination(total, 20, part);
            ViewBag.Pager = pager;

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (lang == "fa")
                ViewBag.PageTitle = "مدیریت منوها";
            else
                ViewBag.PageTitle = "Menu Management";

            ViewBag.Contents = ISystemBaseServ.iPanelMenuService.GetPart(x => x.ParentId == null, pager.StartIndex, pager.PageSize).OrderBy(x => x.Ordering).ToList();

            return View(GetViewName(lang, "Index"));
        }

        [HttpGet]
        public IActionResult Create([FromQuery] string lang)
        {
            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (lang == "fa")
                ViewBag.PageTitle = "ایجاد نقش";
            else
                ViewBag.PageTitle = "Create user role";

            var request = new PanelMenu();
            return View(GetViewName(lang, "Create"), request);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] string lang, PanelMenu request)
        {
            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (string.IsNullOrEmpty(request.Title))
            {
                if (lang == "fa")
                    AddError("نام باید مقدار داشته باشد", "fa");
                else
                    AddError("Title can not be null", "en");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                ViewBag.Messages = Messages;
                return View(GetViewName(lang, "Create"), request);
            }

            request.Enabled = true;
            request.Ordering = ISystemBaseServ.iPanelMenuService.Count(x => x.ParentId == null) + 1;

            ISystemBaseServ.iPanelMenuService.Add(request);
            await ISystemBaseServ.iPanelMenuService.SaveChangesAsync();

            return Redirect("/Panel/PanelMenuManage");

        }


        [HttpGet]
        public IActionResult Edit([FromQuery] string lang, int Id)
        {
            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            var theMenu = ISystemBaseServ.iPanelMenuService.Find(x => x.Id == Id);
            return View(GetViewName(lang, "Edit"), theMenu);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromQuery] string lang, PanelMenu request)
        {
            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (request.Id < 1)
            {
                if (lang == "fa")
                    AddError("خطا در ویرایش لطفا از ابتدا عملیات را انجام دهید", "fa");
                else
                    AddError("Edit feild, please try agan", "en");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                ViewBag.Messages = Messages;
                return View(GetViewName(lang, "Create"), request);
            }

            var theMenu = ISystemBaseServ.iPanelMenuService.Find(x => x.Id == request.Id);
            theMenu.Title = request.Title;
            theMenu.Link = request.Link;
            theMenu.Icon = request.Icon;
            theMenu.Controller = request.Controller;
            theMenu.Roles = request.Roles;
            theMenu.Description = request.Description;
            await ISystemBaseServ.iPanelMenuService.SaveChangesAsync();

            return Redirect("/Panel/PanelMenuManage");
        }


        public async Task<IActionResult> Remove(int Id)
        {
            var theMenu = ISystemBaseServ.iPanelMenuService.Find(x => x.Id == Id);
            ISystemBaseServ.iPanelMenuService.Remove(theMenu);
            await ISystemBaseServ.iPanelMenuService.SaveChangesAsync();
            return Redirect("/Panel/PanelMenuManage");
        }

        public async Task<IActionResult> Enable(int Id)
        {
            var theMenu = ISystemBaseServ.iPanelMenuService.Find(x => x.Id == Id);
            theMenu.Enabled = !theMenu.Enabled;
            await ISystemBaseServ.iPanelMenuService.SaveChangesAsync();
            return Redirect("/Panel/PanelMenuManage");
        }



        public async Task<IActionResult> MenuItems([FromQuery] string lang, int part, int ParentId)
        {
            var parent = await ISystemBaseServ.iPanelMenuService.FindAsync(x => x.Id == ParentId);
            ViewBag.ParentMenu = parent;

            var total = ISystemBaseServ.iPanelMenuService.Count(x => x.ParentId == ParentId);
            var pager = new Pagination(total, 20, part);
            ViewBag.Pager = pager;

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (lang == "fa")
                ViewBag.PageTitle = "مدیریت منوهای " + parent.Title;
            else
                ViewBag.PageTitle = "Sub Menu " + parent.Title;

            ViewBag.Contents = ISystemBaseServ.iPanelMenuService.GetPart(x => x.ParentId == ParentId, pager.StartIndex, pager.PageSize).OrderBy(x => x.Ordering).ToList();

            return View(GetViewName(lang, "MenuItems"));
        }

        [HttpGet]
        public IActionResult CreateItem([FromQuery] string lang, int Id, int ParentId)
        {
            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (lang == "fa")
                ViewBag.PageTitle = "ایجاد منو";
            else
                ViewBag.PageTitle = "Create Menu";

            PanelMenu request;
            if (Id > 0)
            {
                request = ISystemBaseServ.iPanelMenuService.Find(x => x.Id == Id);
            }
            else
            {
                request = new PanelMenu();
                request.ParentId = ParentId;
            }

            return View(GetViewName(lang, "CreateItem"), request);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromQuery] string lang, PanelMenu request)
        {
            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (string.IsNullOrEmpty(request.Title))
            {
                if (lang == "fa")
                    AddError("نام باید مقدار داشته باشد", "fa");
                else
                    AddError("Title can not be null", "en");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                ViewBag.Messages = Messages;
                return View(GetViewName(lang, "Create"), request);
            }


            if (request.Id == 0)
            {
                request.Enabled = true;
                request.Ordering = ISystemBaseServ.iPanelMenuService.Count(x => x.ParentId == null) + 1;
                ISystemBaseServ.iPanelMenuService.Add(request);
            }
            
            await ISystemBaseServ.iPanelMenuService.SaveChangesAsync();

            return Redirect("/Panel/PanelMenuManage/MenuItems?ParentId=" + request.ParentId);

        }

        public async Task<IActionResult> RemoveItem(int Id)
        {
            var theMenu = ISystemBaseServ.iPanelMenuService.Find(x => x.Id == Id);
            int? parentId = theMenu.ParentId;
            ISystemBaseServ.iPanelMenuService.Remove(theMenu);
            await ISystemBaseServ.iPanelMenuService.SaveChangesAsync();
            return Redirect("/Panel/PanelMenuManage/MenuItems?ParentId=" + parentId);
        }

        public async Task<IActionResult> EnableItem(int Id)
        {
            var theMenu = ISystemBaseServ.iPanelMenuService.Find(x => x.Id == Id);
            theMenu.Enabled = !theMenu.Enabled;
            await ISystemBaseServ.iPanelMenuService.SaveChangesAsync();
            return Redirect("/Panel/PanelMenuManage/MenuItems?ParentId=" + theMenu.ParentId);
        }

    }
}
