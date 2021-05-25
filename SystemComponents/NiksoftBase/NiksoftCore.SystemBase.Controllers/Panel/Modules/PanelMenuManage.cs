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

        public IActionResult Index(int part)
        {
            var total = ISystemBaseServ.iPanelMenuService.Count(x => x.ParentId == null);
            var pager = new Pagination(total, 20, part);
            ViewBag.Pager = pager;

            ViewBag.PageTitle = "مدیریت منوها";

            var query = ISystemBaseServ.iPanelMenuService.ExpressionMaker();
            query.Add(x => x.ParentId == null && x.Enabled);

            ViewBag.Contents = ISystemBaseServ.iPanelMenuService.GetPartOptional(query, pager.StartIndex, pager.PageSize).OrderBy(x => x.Ordering).ToList();

            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.PageTitle = "ایجاد نقش";

            var request = new PanelMenu();
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PanelMenu request)
        {

            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("نام باید مقدار داشته باشد", "fa");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            request.Enabled = true;
            request.Ordering = ISystemBaseServ.iPanelMenuService.Count(x => x.ParentId == null) + 1;

            ISystemBaseServ.iPanelMenuService.Add(request);
            await ISystemBaseServ.iPanelMenuService.SaveChangesAsync();

            return Redirect("/Panel/PanelMenuManage");

        }


        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var theMenu = ISystemBaseServ.iPanelMenuService.Find(x => x.Id == Id);
            return View(theMenu);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PanelMenu request)
        {
            if (request.Id < 1)
            {
                AddError("خطا در ویرایش لطفا از ابتدا عملیات را انجام دهید", "fa");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                ViewBag.Messages = Messages;
                return View(request);
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



        public async Task<IActionResult> MenuItems(int part, int ParentId)
        {
            var parent = await ISystemBaseServ.iPanelMenuService.FindAsync(x => x.Id == ParentId);
            ViewBag.ParentMenu = parent;

            var total = ISystemBaseServ.iPanelMenuService.Count(x => x.ParentId == ParentId);
            var pager = new Pagination(total, 20, part);
            ViewBag.Pager = pager;

            ViewBag.PageTitle = "مدیریت منوهای " + parent.Title;

            ViewBag.Contents = ISystemBaseServ.iPanelMenuService.GetPart(x => x.ParentId == ParentId, pager.StartIndex, pager.PageSize).OrderBy(x => x.Ordering).ToList();

            return View();
        }

        [HttpGet]
        public IActionResult CreateItem(int Id, int ParentId)
        {
            ViewBag.PageTitle = "ایجاد منو";

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

            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(PanelMenu request)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("نام باید مقدار داشته باشد", "fa");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            PanelMenu item = new PanelMenu();
            if (request.Id > 0)
            {
                item = ISystemBaseServ.iPanelMenuService.Find(x => x.Id == request.Id);
            }

            item.Title = request.Title;
            item.Link = request.Link;
            item.Icon = request.Icon;
            item.Controller = request.Controller;
            item.Roles = request.Roles;
            item.Description = request.Description;
            item.ParentId = request.ParentId;

            if (request.Id == 0)
            {
                item.Enabled = true;
                item.Ordering = ISystemBaseServ.iPanelMenuService.Count(x => x.ParentId == request.ParentId) + 1;
                ISystemBaseServ.iPanelMenuService.Add(item);
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

        public async Task<IActionResult> OrderDownMenu(int Id)
        {
            var theItem = ISystemBaseServ.iPanelMenuService.Find(x => x.Id == Id);
            var itemsCount = ISystemBaseServ.iPanelMenuService.Count(x => x.ParentId == theItem.ParentId);
            if (theItem.Ordering == itemsCount)
            {
                if (theItem.ParentId == null)
                {
                    return Redirect("/Panel/PanelMenuManage");
                }
                else
                {
                    return Redirect("/Panel/PanelMenuManage/MenuItems/?ParentId=" + theItem.ParentId);
                }
                
            }

            var upItem = ISystemBaseServ.iPanelMenuService.Find(x => x.ParentId == theItem.ParentId && x.Ordering == (theItem.Ordering + 1));

            theItem.Ordering = theItem.Ordering + 1;
            upItem.Ordering = upItem.Ordering - 1;

            await ISystemBaseServ.iPanelMenuService.SaveChangesAsync();

            if (theItem.ParentId == null)
            {
                return Redirect("/Panel/PanelMenuManage");
            }
            else
            {
                return Redirect("/Panel/PanelMenuManage/MenuItems/?ParentId=" + theItem.ParentId);
            }
        }

        public async Task<IActionResult> OrderUpMenu(int Id)
        {
            var theItem = ISystemBaseServ.iPanelMenuService.Find(x => x.Id == Id);
            if (theItem.Ordering == 1)
            {
                if (theItem.ParentId == null)
                {
                    return Redirect("/Panel/PanelMenuManage");
                }
                else
                {
                    return Redirect("/Panel/PanelMenuManage/MenuItems/?ParentId=" + theItem.ParentId);
                }
            }

            var downItem = ISystemBaseServ.iPanelMenuService.Find(x => x.ParentId == theItem.ParentId && x.Ordering == (theItem.Ordering - 1));

            theItem.Ordering = theItem.Ordering - 1;
            downItem.Ordering = downItem.Ordering + 1;

            await ISystemBaseServ.iPanelMenuService.SaveChangesAsync();
            if (theItem.ParentId == null)
            {
                return Redirect("/Panel/PanelMenuManage");
            }
            else
            {
                return Redirect("/Panel/PanelMenuManage/MenuItems/?ParentId=" + theItem.ParentId);
            }
        }

    }
}
