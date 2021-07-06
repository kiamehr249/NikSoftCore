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
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.SystemBase.Controllers.Panel.Modules
{
    [Area("Panel")]
    [Authorize]
    public class CatContentManage : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly IWebHostEnvironment hosting;

        public CatContentManage(IConfiguration Configuration, IWebHostEnvironment hostingEnvironment,
            UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            hosting = hostingEnvironment;
        }

        public IActionResult Index(ContentGridRequest request)
        {
            var query = ISystemBaseServ.iGeneralContentServ.ExpressionMaker();

            query.Add(x => x.CategoryId == request.CategoryId);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = ISystemBaseServ.iGeneralContentServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.PageTitle = "Content Manage";

            ViewBag.Contents = ISystemBaseServ.iGeneralContentServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View(request);
        }

        [HttpGet]
        public IActionResult Create(int CatId, int Id)
        {
            ViewBag.PageTitle = "Create Content";

            var request = new ContentRequest();
            if (Id > 0)
            {
                var theItem = ISystemBaseServ.iGeneralContentServ.Find(x => x.Id == Id);
                request.Id = theItem.Id;
                request.Title = theItem.Title;
                request.KeyValue = theItem.KeyValue;
                request.Header = theItem.Header;
                request.BodyText = theItem.BodyText;
                request.Footer = theItem.Footer;
                request.Icon = theItem.Icon;
                request.Image = theItem.Image;
                request.SmallImage = theItem.SmallImage;
            }

            request.CategoryId = CatId;


            DropDownBinder(request);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContentRequest request)
        {
            ViewBag.PageTitle = "Create Content";
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (!FormVlide(request))
            {
                DropDownBinder(request);
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
                    UnitPath = Config.GetSection("FileRoot:ContentDir").Value
                });

                if (!SaveImage.Success)
                {
                    DropDownBinder(request);
                    AddError("File upload failed Try again");
                    ViewBag.Messages = Messages;
                    return View(request);
                }

                Image = SaveImage.FilePath;
            }

            string smallImage = string.Empty;
            if (request.SmallFile != null && request.SmallFile.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.SmallFile,
                    RootPath = hosting.ContentRootPath,
                    UnitPath = Config.GetSection("FileRoot:ContentDir").Value
                });

                if (!SaveImage.Success)
                {
                    DropDownBinder(request);
                    AddError("File upload failed Try again");
                    ViewBag.Messages = Messages;
                    return View(request);
                }

                smallImage = SaveImage.FilePath;
            }

            var item = new GeneralContent();
            if (request.Id > 0)
            {
                item = await ISystemBaseServ.iGeneralContentServ.FindAsync(x => x.Id == request.Id);
                item.EditedBy = user.Id;
                item.EditDate = DateTime.Now;
            }

            item.Title = request.Title;
            item.KeyValue = request.KeyValue;
            item.Header = request.Header;
            item.BodyText = request.BodyText;
            item.Footer = request.Footer;
            item.Icon = request.Icon;

            if (!string.IsNullOrEmpty(Image))
                item.Image = Image;

            if (!string.IsNullOrEmpty(smallImage))
                item.SmallImage = smallImage;

            if (request.Id == 0)
            {
                item.CategoryId = request.CategoryId;
                item.CreatedBy = user.Id;
                item.CreateDate = DateTime.Now;
                ISystemBaseServ.iGeneralContentServ.Add(item);
            }

            await ISystemBaseServ.iContentCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/CatContentManage/?CategoryId=" + request.CategoryId);

        }


        public async Task<IActionResult> Remove(int Id)
        {
            var theContent = ISystemBaseServ.iGeneralContentServ.Find(x => x.Id == Id);
            var catId = theContent.CategoryId;
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
            return Redirect("/Panel/CatContentManage/?CategoryId=" + catId);
        }

        public async Task<IActionResult> Enable(int Id)
        {
            var theContent = ISystemBaseServ.iGeneralContentServ.Find(x => x.Id == Id);
            //theContent.Enabled = !theContent.Enabled;
            await ISystemBaseServ.iGeneralContentServ.SaveChangesAsync();
            return Redirect("/Panel/CatContentManage/?CategoryId=" + theContent.CategoryId);
        }

        private void DropDownBinder(ContentRequest request)
        {
            var categories = ISystemBaseServ.iContentCategoryServ.GetAll(x => true);
            ViewBag.Categories = new SelectList(categories, "Id", "Title", request?.CategoryId);
        }

        private bool FormVlide(ContentRequest request)
        {
            bool result = true;
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("The title must have a value", "fa");
                result = false;
            }

            return result;
        }
    }
}
