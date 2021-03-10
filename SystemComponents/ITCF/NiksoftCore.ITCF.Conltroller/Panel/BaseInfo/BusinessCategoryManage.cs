using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.ITCF.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.ITCF.Conltroller.Panel.BaseInfo
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin,Admin")]
    public class BusinessCategoryManage : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        public IITCFService iITCFServ { get; set; }
        private readonly IWebHostEnvironment hosting;

        public BusinessCategoryManage(IConfiguration Configuration, IWebHostEnvironment hostingEnvironment,
            UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            iITCFServ = new ITCFService(Configuration);
            hosting = hostingEnvironment;
        }

        public IActionResult Index([FromQuery] string lang, int part)
        {
            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            var total = iITCFServ.IBusinessCategoryServ.Count(x => true);
            var pager = new Pagination(total, 20, part);
            ViewBag.Pager = pager;



            if (lang == "fa")
                ViewBag.PageTitle = "مدیریت دسته بندی ها";
            else
                ViewBag.PageTitle = "Business Category Management";

            ViewBag.Contents = iITCFServ.IBusinessCategoryServ.GetPart(x => true, pager.StartIndex, pager.PageSize).ToList();

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
                ViewBag.PageTitle = "ایجاد دسته بندی";
            else
                ViewBag.PageTitle = "Create Business Category";

            var request = new BusinessCategoryRequest();
            DropDownBinder(request);
            return View(GetViewName(lang, "Create"), request);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] string lang,[FromForm] BusinessCategoryRequest request)
        {
            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

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

            var newCat = new BusinessCategory
            {
                Title = request.Title,
                EnTitle = request.EnTitle,
                ArTitle = request .ArTitle,
                Description = request.Description,
                EnDescription = request.EnDescription,
                ArDescription = request.ArDescription,
                Icone = request.Icone,
                Image = Image,
                ParentId = request.ParentId == 0 ? null : request.ParentId
            };

            iITCFServ.IBusinessCategoryServ.Add(newCat);
            await iITCFServ.IBusinessCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/BusinessCategoryManage");

        }

        [HttpGet]
        public IActionResult Edit([FromQuery] string lang, int Id)
        {
            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (lang == "fa")
                ViewBag.PageTitle = "بروزرسانی دسته بندی";
            else
                ViewBag.PageTitle = "Update Business Category";

            var catItem = iITCFServ.IBusinessCategoryServ.Find(x => x.Id == Id);
            var request = new BusinessCategoryRequest
            {
                Id = catItem.Id,
                Title = catItem.Title,
                EnTitle = catItem.EnTitle,
                ArTitle = catItem.ArTitle,
                Description = catItem.Description,
                EnDescription = catItem.EnDescription,
                ArDescription = catItem.ArDescription,
                Icone = catItem.Icone,
                Image = catItem.Image,
                ParentId = catItem.ParentId
            };
            DropDownBinder(request);
            return View(GetViewName(lang, "Edit"), request);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromQuery] string lang, [FromForm] BusinessCategoryRequest request)
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

            if (!FormVlide(lang, request))
            {
                DropDownBinder(request);
                ViewBag.Messages = Messages;
                return View(GetViewName(lang, "Create"), request);
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
                    return View(GetViewName(lang, "Create"), request);
                }

                imageEdit = Image.FilePath;
            }

            

            var theContent = iITCFServ.IBusinessCategoryServ.Find(x => x.Id == request.Id);
            theContent.Title = request.Title;
            theContent.EnTitle = request.EnTitle;
            theContent.ArTitle = request.ArTitle;
            theContent.Description = request.Description;
            theContent.EnDescription = request.EnDescription;
            theContent.ArDescription = request.ArDescription;
            theContent.Icone = request.Icone;
            if (!string.IsNullOrEmpty(imageEdit))
                theContent.Image = imageEdit;
            theContent.ParentId = request.ParentId == 0 ? null : request.ParentId;
            await iITCFServ.IBusinessCategoryServ.SaveChangesAsync();

            return Redirect("/Panel/BusinessCategoryManage");
        }


        public async Task<IActionResult> Remove(int Id)
        {
            var theContent = iITCFServ.IBusinessCategoryServ.Find(x => x.Id == Id);
            if (!string.IsNullOrEmpty(theContent.Image))
            {
                NikTools.RemoveFile(new RemoveFileRequest
                {
                    RootPath = hosting.ContentRootPath,
                    FilePath = theContent.Image
                });
            }
            
            iITCFServ.IBusinessCategoryServ.Remove(theContent);
            await iITCFServ.IBusinessCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/BusinessCategoryManage");
        }

        public async Task<IActionResult> Enable(int Id)
        {
            var theContent = iITCFServ.IBusinessCategoryServ.Find(x => x.Id == Id);
            //theContent.Enabled = !theContent.Enabled;
            await iITCFServ.IBusinessCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/IndParkManage");
        }

        private void DropDownBinder(BusinessCategoryRequest request)
        {
            var query1 = iITCFServ.IBusinessCategoryServ.ExpressionMaker();
            query1.Add(x => true);
            if (request.Id != 0)
            {
                query1.Add(x => x.Id != request.Id);
            }
            var categories = iITCFServ.IBusinessCategoryServ.GetAll(query1);
            ViewBag.Parents = new SelectList(categories, "Id", "Title", request?.ParentId);

            //var provinces = iITCFServ.iProvinceServ.GetPart(x => true, 0, 40);
            //ViewBag.Province = new SelectList(provinces, "Id", "Title", request?.CountryId);

            //var cities = iITCFServ.iProvinceServ.GetPart(x => true, 0, 40);
            //ViewBag.Province = new SelectList(provinces, "Id", "Title", request?.CountryId);
        }

        private bool FormVlide(string lang, BusinessCategoryRequest request)
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
