using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.SystemBase.Service;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.SystemBase.Controllers.Panel.Modules
{
    [Area("Panel")]
    [Authorize]
    public class MenuManage : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly IWebHostEnvironment hosting;

        public MenuManage(IConfiguration Configuration, IWebHostEnvironment hostingEnvironment,
            UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            hosting = hostingEnvironment;
        }

        public IActionResult Index(MenuCategoryGridRequest request)
        {

            var query = ISystemBaseServ.iMenuCategoryServ.ExpressionMaker();

            query.Add(x => true);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = ISystemBaseServ.iMenuCategoryServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.PageTitle = "مدیریت منو ها";

            ViewBag.Contents = ISystemBaseServ.iMenuCategoryServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();

            return View();
        }

        [HttpGet]
        public IActionResult Create(int Id)
        {

            ViewBag.PageTitle = "ایجاد دسته بندی";

            var request = new MenuCategoryRequest();

            if (Id > 0)
            {
                var item = ISystemBaseServ.iMenuCategoryServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.KeyValue = item.KeyValue;
                request.Description = item.Description;
                request.Image = item.Image;
                request.Enabled = item.Enabled;
            }


            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] MenuCategoryRequest request)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (!CatFormValide(request))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            string Image = string.Empty;
            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.ImageFile,
                    RootPath = hosting.ContentRootPath,
                    UnitPath = Config.GetSection("FileRoot:BaseSystem").Value
                });

                if (!SaveImage.Success)
                {
                    Messages.Add(new NikMessage
                    {
                        Message = "آپلود فایل انجام نشد مجدد تلاش کنید",
                        Type = MessageType.Error,
                        Language = "Fa"
                    });
                    ViewBag.Messages = Messages;
                    return View(request);
                }

                Image = SaveImage.FilePath;
            }

            MenuCategory item;

            if (request.Id > 0)
            {
                item = ISystemBaseServ.iMenuCategoryServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }
            else
            {
                item = new MenuCategory();
            }

            item.Title = request.Title;
            item.KeyValue = request.KeyValue;
            item.Description = request.Description;
            if (!string.IsNullOrEmpty(Image))
                item.Image = Image;


            if (request.Id == 0)
            {
                item.Enabled = true;
                item.CreatedBy = user.Id;
                item.CreateDate = DateTime.Now;
                ISystemBaseServ.iMenuCategoryServ.Add(item);
            }


            await ISystemBaseServ.iMenuCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/MenuManage");

        }

        public async Task<IActionResult> Remove(int Id)
        {
            var theContent = ISystemBaseServ.iMenuCategoryServ.Find(x => x.Id == Id);
            if (!string.IsNullOrEmpty(theContent.Image))
            {
                NikTools.RemoveFile(new RemoveFileRequest
                {
                    RootPath = hosting.ContentRootPath,
                    FilePath = theContent.Image
                });
            }

            ISystemBaseServ.iMenuCategoryServ.Remove(theContent);
            await ISystemBaseServ.iMenuCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/MenuManage");
        }

        public async Task<IActionResult> Enable(int Id)
        {
            var theContent = ISystemBaseServ.iMenuCategoryServ.Find(x => x.Id == Id);
            theContent.Enabled = !theContent.Enabled;
            await ISystemBaseServ.iMenuCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/MenuManage");
        }

        private void DropDownBinder(ContentRequest request)
        {
            var categories = ISystemBaseServ.iContentCategoryServ.GetAll(x => true);
            ViewBag.Categories = new SelectList(categories, "Id", "Title", request?.CategoryId);
        }

        private bool CatFormValide(MenuCategoryRequest request)
        {
            bool result = true;
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("عنوان باید مقدار داشته باشد", "fa");
                result = false;
            }

            return result;
        }


        public IActionResult MenuGrid(MenuGridRequest request)
        {
            var category = ISystemBaseServ.iMenuCategoryServ.Find(x => x.Id == request.CategoryId);
            ViewBag.Category = category;

            Menu parent = new Menu();
            if (request.ParentId > 0)
            {
                parent = ISystemBaseServ.iMenuServ.Find(x => x.Id == request.ParentId);
            }

            ViewBag.Parent = parent;

            var query = ISystemBaseServ.iMenuServ.ExpressionMaker();

            query.Add(x => x.CategoryId == request.CategoryId);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }
            ViewBag.Search = isSearch;

            if (request.ParentId != 0)
                query.Add(x => x.ParentId == request.ParentId);
            else
                query.Add(x => x.ParentId == null);

            var total = ISystemBaseServ.iMenuServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.PageTitle = "مدیریت منو / " + category.Title;

            ViewBag.Contents = ISystemBaseServ.iMenuServ.GetPart(query, pager.StartIndex, pager.PageSize, x => x.OrderId, true).ToList();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateMenu(int Id, int ParentId, int CategoryId)
        {

            var category = await ISystemBaseServ.iMenuCategoryServ.FindAsync(x => x.Id == CategoryId);

            Menu parent = new Menu();
            if (ParentId > 0)
            {
                parent = await ISystemBaseServ.iMenuServ.FindAsync(x => x.Id == ParentId);
            }

            ViewBag.PageTitle = category.Title + " / ایجاد آیتم ها / " + parent.Title;

            var request = new MenuRequest();

            if (Id > 0)
            {
                var item = ISystemBaseServ.iMenuServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.Link = item.Link;
                request.Description = item.Description;
                request.Image = item.Image;
                request.Enabled = item.Enabled;
                request.OrderId = item.OrderId;
                request.CategoryId = item.CategoryId;
                request.ParentId = item.ParentId != null ? item.ParentId.Value : 0;
            }
            else
            {
                request.CategoryId = CategoryId;
                request.ParentId = ParentId;
            }


            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromForm] MenuRequest request)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (!MenuFormValide(request))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            string Image = string.Empty;
            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.ImageFile,
                    RootPath = hosting.ContentRootPath,
                    UnitPath = Config.GetSection("FileRoot:BaseSystem").Value
                });

                if (!SaveImage.Success)
                {
                    Messages.Add(new NikMessage
                    {
                        Message = "آپلود فایل انجام نشد مجدد تلاش کنید",
                        Type = MessageType.Error,
                        Language = "Fa"
                    });
                    ViewBag.Messages = Messages;
                    return View(request);
                }

                Image = SaveImage.FilePath;
            }

            Menu item;

            if (request.Id > 0)
            {
                item = ISystemBaseServ.iMenuServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }
            else
            {
                item = new Menu();
            }

            item.Title = request.Title;
            item.Link = request.Link;
            item.Description = request.Description;
            if (!string.IsNullOrEmpty(Image))
                item.Image = Image;

            item.CategoryId = request.CategoryId;
            item.ParentId = request.ParentId == 0 ? null : request.ParentId;


            if (request.Id == 0)
            {
                var query = ISystemBaseServ.iMenuServ.ExpressionMaker();
                query.Add(x => x.CategoryId == request.CategoryId);

                if (request.ParentId > 0)
                    query.Add(x => x.ParentId == request.ParentId);
                else
                    query.Add(x => x.ParentId == null);

                var maxOrder = ISystemBaseServ.iMenuServ.Count(query);
                item.OrderId = maxOrder + 1;
                item.Enabled = true;
                item.CreatedBy = user.Id;
                item.CreateDate = DateTime.Now;
                ISystemBaseServ.iMenuServ.Add(item);
            }

            int? parentId = null;
            if (request.ParentId > 0)
            {
                parentId = request.ParentId;
            }


            await ISystemBaseServ.iMenuServ.SaveChangesAsync();
            return Redirect("/Panel/MenuManage/MenuGrid?CategoryId=" + request.CategoryId + "&ParentId=" + parentId);

        }

        public async Task<IActionResult> RemoveMenu(int Id)
        {
            var theContent = ISystemBaseServ.iMenuServ.Find(x => x.Id == Id);
            var catId = theContent.CategoryId;
            var parentId = theContent.ParentId;
            if (!string.IsNullOrEmpty(theContent.Image))
            {
                NikTools.RemoveFile(new RemoveFileRequest
                {
                    RootPath = hosting.ContentRootPath,
                    FilePath = theContent.Image
                });
            }

            ISystemBaseServ.iMenuServ.Remove(theContent);
            await ISystemBaseServ.iMenuServ.SaveChangesAsync();
            return Redirect("/Panel/MenuManage/MenuGrid?CategoryId=" + catId + "&ParentId=" + parentId);
        }

        public async Task<IActionResult> EnableMenu(int Id)
        {
            var theContent = ISystemBaseServ.iMenuServ.Find(x => x.Id == Id);
            theContent.Enabled = !theContent.Enabled;
            await ISystemBaseServ.iMenuServ.SaveChangesAsync();
            return Redirect("/Panel/MenuManage/MenuGrid?CategoryId=" + theContent.CategoryId + "&ParentId=" + theContent.ParentId);
        }

        public async Task<IActionResult> OrderUpMenu(int Id)
        {
            var theItem = ISystemBaseServ.iMenuServ.Find(x => x.Id == Id);
            var itemsCount = ISystemBaseServ.iMenuServ.Count(x => x.CategoryId == theItem.CategoryId && x.ParentId == theItem.ParentId);
            if (theItem.OrderId == itemsCount)
            {
                return Redirect("/Panel/MenuManage/MenuGrid?CategoryId=" + theItem.CategoryId + "&ParentId=" + theItem.ParentId);
            }

            var upItem = ISystemBaseServ.iMenuServ.Find(x => x.CategoryId == theItem.CategoryId && x.ParentId == theItem.ParentId && x.OrderId == (theItem.OrderId + 1));

            theItem.OrderId = theItem.OrderId + 1;
            upItem.OrderId = upItem.OrderId - 1;

            await ISystemBaseServ.iMenuServ.SaveChangesAsync();
            return Redirect("/Panel/MenuManage/MenuGrid?CategoryId=" + theItem.CategoryId + "&ParentId=" + theItem.ParentId);
        }

        public async Task<IActionResult> OrderDownMenu(int Id)
        {
            var theItem = ISystemBaseServ.iMenuServ.Find(x => x.Id == Id);
            if (theItem.OrderId == 1)
            {
                return Redirect("/Panel/MenuManage/MenuGrid?CategoryId=" + theItem.CategoryId + "&ParentId=" + theItem.ParentId);
            }

            var upItem = ISystemBaseServ.iMenuServ.Find(x => x.CategoryId == theItem.CategoryId && x.ParentId == theItem.ParentId && x.OrderId == (theItem.OrderId - 1));

            theItem.OrderId = theItem.OrderId - 1;
            upItem.OrderId = upItem.OrderId + 1;

            await ISystemBaseServ.iMenuServ.SaveChangesAsync();
            return Redirect("/Panel/MenuManage/MenuGrid?CategoryId=" + theItem.CategoryId + "&ParentId=" + theItem.ParentId);
        }

        private bool MenuFormValide(MenuRequest request)
        {
            bool result = true;
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("عنوان باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (request.CategoryId == 0)
            {
                AddError("دسته بندی باید مقدار داشته باشد", "fa");
                result = false;
            }

            return result;
        }


    }
}
