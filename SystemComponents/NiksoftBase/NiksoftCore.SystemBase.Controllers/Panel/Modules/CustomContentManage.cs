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
    public class CustomContentManage : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly IWebHostEnvironment hosting;

        public CustomContentManage(IConfiguration Configuration, IWebHostEnvironment hostingEnvironment,
            UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            hosting = hostingEnvironment;
        }

        public IActionResult Index(CategoryGridRequest request)
        {
            if (!string.IsNullOrEmpty(request.lang))
                request.lang = request.lang.ToLower();
            else
                request.lang = defaultLang.ShortName.ToLower();

            var total = ISystemBaseServ.iContentCategoryServ.Count(x => true);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;

            int? parentId = request.ParentId == 0 ? null : request.ParentId;
            ContentCategory Parent = new ContentCategory();
            if (parentId != null && parentId > 0)
            {
                Parent = ISystemBaseServ.iContentCategoryServ.Find(x => x.Id == parentId);
            }

            if (request.lang == "fa")
                ViewBag.PageTitle = "مدیریت محتوا / بخش ها " + Parent.Title;
            else
                ViewBag.PageTitle = "Business Category Management";



            ViewBag.Parent = Parent;

            ViewBag.Contents = ISystemBaseServ.iContentCategoryServ.GetPart(x => x.ParentId == parentId, pager.StartIndex, pager.PageSize).ToList();

            return View(GetViewName(request.lang, "Index"));
        }

        public IActionResult ContentList(ContentGridRequest request)
        {
            if (!string.IsNullOrEmpty(request.lang))
                request.lang = request.lang.ToLower();
            else
                request.lang = defaultLang.ShortName.ToLower();

            var query = ISystemBaseServ.iGeneralContentServ.ExpressionMaker();
            query.Add(x => x.CategoryId == request.CategoryId);

            var total = ISystemBaseServ.iGeneralContentServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;

            var Category = ISystemBaseServ.iContentCategoryServ.Find(x => x.Id == request.CategoryId);

            if (request.lang == "fa")
                ViewBag.PageTitle = "مدیریت محتوا /  " + Category.Title;
            else
                ViewBag.PageTitle = "Business Category Management";

            ViewBag.Caregory = Category;

            ViewBag.Contents = ISystemBaseServ.iGeneralContentServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();

            return View(GetViewName(request.lang, "ContentList"));
        }

        [HttpGet]
        public IActionResult Create([FromQuery] string lang, int Id, int CategoryId)
        {
            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();
            

            var request = new ContentRequest();
            if (Id > 0)
            {
                var item = ISystemBaseServ.iGeneralContentServ.Find(x => x.Id == Id);
                request.Title = item.Title;
                request.Header = item.Header;
                request.BodyText = item.BodyText;
                request.Footer = item.Footer;
                request.Icon = item.Icon;
                request.Image = item.Image;
                request.KeyValue = item.KeyValue;
                request.Link = item.Link;
                request.LinkTitle = item.LinkTitle;
                request.Enabled = item.Enabled;
                request.CategoryId = item.CategoryId;
            }
            else
            {
                request.CategoryId = CategoryId;
            }

            var category = ISystemBaseServ.iContentCategoryServ.Find(x => x.Id == request.CategoryId);
            ViewBag.Category = category;

            if (lang == "fa")
                ViewBag.PageTitle = "ایجاد محتوای " + category.Title;
            else
                ViewBag.PageTitle = "Create Business Category";

            DropDownBinder(request);
            return View(GetViewName(lang, "Create"), request);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] string lang, [FromForm] ContentRequest request)
        {
            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            var category = ISystemBaseServ.iContentCategoryServ.Find(x => x.Id == request.CategoryId);
            ViewBag.Category = category;

            if (lang == "fa")
                ViewBag.PageTitle = "ایجاد محتوای " + category.Title;
            else
                ViewBag.PageTitle = "Create Business Category";

            if (!FormVlide(lang, request))
            {
                DropDownBinder(request);
                ViewBag.Messages = Messages;
                return View(GetViewName(lang, "Create"), request);
            }

            string Image = string.Empty;
            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.ImageFile,
                    RootPath = hosting.ContentRootPath,
                    UnitPath = Config.GetSection("FileRoot:BusinessFile").Value
                });

                if (!SaveImage.Success)
                {
                    DropDownBinder(request);
                    Messages.Add(new NikMessage
                    {
                        Message = "آپلود فایل انجام نشد مجدد تلاش کنید",
                        Type = MessageType.Error,
                        Language = "Fa"
                    });
                    ViewBag.Messages = Messages;
                    return View(GetViewName(lang, "Create"), request);
                }

                Image = SaveImage.FilePath;
            }

            GeneralContent item;

            if (request.Id > 0)
            {
                item = await ISystemBaseServ.iGeneralContentServ.FindAsync(x => x.Id == request.Id);
            }
            else
            {
                item = new GeneralContent();
            }

            item.Title = request.Title;
            item.KeyValue = request.KeyValue;
            item.Header = request.Header;
            item.BodyText = request.BodyText;
            item.Footer = request.Footer;
            item.Icon = request.Icon;
            if (!string.IsNullOrEmpty(Image))
            {
                item.Image = Image;
            }
            
            item.Link = request.Link;
            item.LinkTitle = request.LinkTitle;
            item.CategoryId = request.CategoryId;

            if (request.Id == 0)
            {
                ISystemBaseServ.iGeneralContentServ.Add(item);
            }

            
            await ISystemBaseServ.iContentCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/CustomContentManage/ContentList?CategoryId=" + request.CategoryId);

        }


        public async Task<IActionResult> Remove(int Id)
        {
            var theContent = ISystemBaseServ.iGeneralContentServ.Find(x => x.Id == Id);
            var catgeoryId = theContent.CategoryId;
            if (!string.IsNullOrEmpty(theContent.Image))
            {
                NikTools.RemoveFile(new RemoveFileRequest
                {
                    RootPath = hosting.ContentRootPath,
                    FilePath = theContent.Image
                });
            }

            ISystemBaseServ.iGeneralContentServ.Remove(theContent);
            await ISystemBaseServ.iGeneralContentServ.SaveChangesAsync();
            return Redirect("/Panel/CustomContentManage/ContentList?CategoryId=" + catgeoryId);
        }

        public async Task<IActionResult> Enable(int Id)
        {
            var theContent = ISystemBaseServ.iGeneralContentServ.Find(x => x.Id == Id);
            theContent.Enabled = !theContent.Enabled;
            await ISystemBaseServ.iGeneralContentServ.SaveChangesAsync();
            return Redirect("/Panel/CustomContentManage/ContentList?CategoryId=" + theContent.CategoryId);
        }

        private void DropDownBinder(ContentRequest request)
        {
            var categories = ISystemBaseServ.iContentCategoryServ.GetAll(x => true);
            ViewBag.Categories = new SelectList(categories, "Id", "Title", request?.CategoryId);
        }

        private bool FormVlide(string lang, ContentRequest request)
        {
            bool result = true;
            if (string.IsNullOrEmpty(request.Title))
            {
                if (lang == "fa")
                    AddError("عنوان باید مقدار داشته باشد", "fa");
                else
                    AddError("Title can not be null", "en");
                result = false;
            }

            return result;
        }
    }
}
