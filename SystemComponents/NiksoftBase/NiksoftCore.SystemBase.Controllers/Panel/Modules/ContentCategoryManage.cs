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
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.SystemBase.Controllers.Panel.Modules
{
    [Area("Panel")]
    [Authorize]
    public class ContentCategoryManage : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly IWebHostEnvironment hosting;

        public ContentCategoryManage(IConfiguration Configuration, IWebHostEnvironment hostingEnvironment,
            UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            hosting = hostingEnvironment;
        }

        public IActionResult Index(int part)
        {
            var query = ISystemBaseServ.iContentCategoryServ.ExpressionMaker();
            query.Add(x => true);

            var total = ISystemBaseServ.iContentCategoryServ.Count(query);
            var pager = new Pagination(total, 10, part);
            ViewBag.Pager = pager;

            ViewBag.PageTitle = "مدیریت دسته بندی ها";
            ViewBag.Contents = ISystemBaseServ.iContentCategoryServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();

            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.PageTitle = "ایجاد دسته بندی";
            var request = new ContentCategoryRequest();
            DropDownBinder(request);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ContentCategoryRequest request)
        {

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
                    return View(request);
                }

                Image = SaveImage.FilePath;
            }

            var newCat = new ContentCategory
            {
                Title = request.Title,
                KeyValue = request.KeyValue,
                Description = request.Description,
                Icon = request.Icon,
                Image = Image,
                ParentId = request.ParentId == 0 ? null : request.ParentId
            };

            ISystemBaseServ.iContentCategoryServ.Add(newCat);
            await ISystemBaseServ.iContentCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/ContentCategoryManage");

        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            ViewBag.PageTitle = "بروزرسانی دسته بندی";

            var theItem = ISystemBaseServ.iContentCategoryServ.Find(x => x.Id == Id);
            var request = new ContentCategoryRequest
            {
                Id = theItem.Id,
                Title = theItem.Title,
                KeyValue = theItem.KeyValue,
                Description = theItem.Description,
                Icon = theItem.Icon,
                Image = theItem.Image,
                ParentId = theItem.ParentId
            };
            DropDownBinder(request);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] ContentCategoryRequest request)
        {
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



            var theContent = ISystemBaseServ.iContentCategoryServ.Find(x => x.Id == request.Id);
            theContent.Title = request.Title;
            theContent.KeyValue = request.KeyValue;
            theContent.Description = request.Description;
            theContent.Icon = request.Icon;
            if (!string.IsNullOrEmpty(imageEdit))
                theContent.Image = imageEdit;
            theContent.ParentId = request.ParentId == 0 ? null : request.ParentId;
            await ISystemBaseServ.iContentCategoryServ.SaveChangesAsync();

            return Redirect("/Panel/ContentCategoryManage");
        }


        public async Task<IActionResult> Remove(int Id)
        {
            var theContent = ISystemBaseServ.iContentCategoryServ.Find(x => x.Id == Id);
            if (!string.IsNullOrEmpty(theContent.Image))
            {
                NikTools.RemoveFile(new RemoveFileRequest
                {
                    RootPath = hosting.ContentRootPath,
                    FilePath = theContent.Image
                });
            }

            ISystemBaseServ.iContentCategoryServ.Remove(theContent);
            await ISystemBaseServ.iContentCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/ContentCategoryManage");
        }

        public async Task<IActionResult> Enable(int Id)
        {
            var theContent = ISystemBaseServ.iContentCategoryServ.Find(x => x.Id == Id);
            //theContent.Enabled = !theContent.Enabled;
            await ISystemBaseServ.iContentCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/ContentCategoryManage");
        }

        private void DropDownBinder(ContentCategoryRequest request)
        {
            var query1 = ISystemBaseServ.iContentCategoryServ.ExpressionMaker();
            query1.Add(x => true);
            if (request.Id != 0)
            {
                query1.Add(x => x.Id != request.Id);
            }
            var categories = ISystemBaseServ.iContentCategoryServ.GetAll(query1);
            ViewBag.Parents = new SelectList(categories, "Id", "Title", request?.ParentId);
        }

        private bool FormVlide(ContentCategoryRequest request)
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
