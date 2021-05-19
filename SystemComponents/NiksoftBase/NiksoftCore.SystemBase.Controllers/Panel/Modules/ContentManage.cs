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
    public class ContentManage : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly IWebHostEnvironment hosting;

        public ContentManage(IConfiguration Configuration, IWebHostEnvironment hostingEnvironment,
            UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            hosting = hostingEnvironment;
        }

        public IActionResult Index(int part)
        {
            var total = ISystemBaseServ.iGeneralContentServ.Count(x => true);
            var pager = new Pagination(total, 20, part);
            ViewBag.Pager = pager;

            ViewBag.PageTitle = "مدیریت دسته بندی ها";

            ViewBag.Contents = ISystemBaseServ.iGeneralContentServ.GetPart(x => true, pager.StartIndex, pager.PageSize).ToList();

            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.PageTitle = "ایجاد دسته بندی";

            var request = new ContentRequest();
            DropDownBinder(request);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ContentRequest request)
        {
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
                    UnitPath = Config.GetSection("FileRoot:MarketerFiles").Value
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
                    return View(request);
                }

                Image = SaveImage.FilePath;
            }

            var newItem = new GeneralContent
            {
                Title = request.Title,
                KeyValue = request.KeyValue,
                Header = request.Header,
                BodyText = request.BodyText,
                Footer = request.Footer,
                Icon = request.Icon,
                Image = Image,
                CategoryId = request.CategoryId,
                CreatedBy = user.Id,
                CreateDate = DateTime.Now
            };

            ISystemBaseServ.iGeneralContentServ.Add(newItem);
            await ISystemBaseServ.iContentCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/ContentManage");

        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            ViewBag.PageTitle = "بروزرسانی دسته بندی";

            var theItem = ISystemBaseServ.iGeneralContentServ.Find(x => x.Id == Id);
            var request = new ContentRequest
            {
                Id = theItem.Id,
                Title = theItem.Title,
                KeyValue = theItem.KeyValue,
                Header = theItem.Header,
                BodyText = theItem.BodyText,
                Footer = theItem.Footer,
                Icon = theItem.Icon,
                Image = theItem.Image,
                CategoryId = theItem.CategoryId
            };
            DropDownBinder(request);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] ContentRequest request)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (request.Id < 1)
            {
                AddError("خطا در ویرایش لطفا از ابتدا عملیات را انجام دهید", "fa");
            }

            if (!FormVlide(request))
            {
                DropDownBinder(request);
                ViewBag.Messages = Messages;
                return View(request);
            }

            string imageEdit = string.Empty;
            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                var Image = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.ImageFile,
                    RootPath = hosting.ContentRootPath,
                    UnitPath = Config.GetSection("FileRoot:BusinessFile").Value
                });

                if (!Image.Success)
                {
                    DropDownBinder(request);
                    Messages.Add(new NikMessage
                    {
                        Message = "آپلود فایل انجام نشد مجدد تلاش کنید",
                        Type = MessageType.Error,
                        Language = "Fa"
                    });
                    ViewBag.Messages = Messages;
                    return View(request);
                }

                imageEdit = Image.FilePath;
            }



            var theContent = ISystemBaseServ.iGeneralContentServ.Find(x => x.Id == request.Id);
            theContent.Title = request.Title;
            theContent.KeyValue = request.KeyValue;
            theContent.Header = request.Header;
            theContent.BodyText = request.BodyText;
            theContent.Footer = request.Footer;
            theContent.Icon = request.Icon;
            if (!string.IsNullOrEmpty(imageEdit))
                theContent.Image = imageEdit;
            theContent.CategoryId = request.CategoryId;
            theContent.EditedBy = user.Id;
            theContent.EditDate = DateTime.Now;
            await ISystemBaseServ.iGeneralContentServ.SaveChangesAsync();

            return Redirect("/Panel/ContentManage");
        }


        public async Task<IActionResult> Remove(int Id)
        {
            var theContent = ISystemBaseServ.iGeneralContentServ.Find(x => x.Id == Id);
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
            return Redirect("/Panel/ContentManage");
        }

        public async Task<IActionResult> Enable(int Id)
        {
            var theContent = ISystemBaseServ.iGeneralContentServ.Find(x => x.Id == Id);
            //theContent.Enabled = !theContent.Enabled;
            await ISystemBaseServ.iGeneralContentServ.SaveChangesAsync();
            return Redirect("/Panel/ContentManage");
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
                AddError("عنوان باید مقدار داشته باشد", "fa");
                result = false;
            }

            return result;
        }
    }
}
