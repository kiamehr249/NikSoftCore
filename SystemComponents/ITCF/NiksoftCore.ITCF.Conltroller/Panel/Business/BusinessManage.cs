using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.ITCF.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.ITCF.Conltroller.Panel.Business
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin,Admin")]
    public class BusinessManage : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        public IITCFService iITCFServ { get; set; }
        private readonly IWebHostEnvironment hosting;

        public BusinessManage(IConfiguration Configuration, IWebHostEnvironment hostingEnvironment,
            UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            iITCFServ = new ITCFService(Configuration);
            hosting = hostingEnvironment;
        }

        public async Task<IActionResult> Index([FromQuery] string lang, int part)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            var total = iITCFServ.IBusinessServ.Count(x => x.CreatorId == theUser.Id);
            var pager = new Pagination(total, 20, part);
            ViewBag.Pager = pager;

            if (lang == "fa")
                ViewBag.PageTitle = "مدیریت کسب و کار";
            else
                ViewBag.PageTitle = "Business Management";

            ViewBag.Contents = iITCFServ.IBusinessServ.GetPart(x => x.CreatorId == theUser.Id, pager.StartIndex, pager.PageSize).ToList();

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
                ViewBag.PageTitle = "ایجاد کسب و کار";
            else
                ViewBag.PageTitle = "Create Business Category";

            var request = new BusinessRequest();
            request.ProvinceId = 0;
            request.IndustrialParkId = 0;
            request.CatgoryId = 0;
            request.CountryId = 1;
            DropDownBinder(request);
            return View(GetViewName(lang, "Create"), request);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] string lang, [FromForm] BusinessRequest request)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);

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

            string logo = "";
            if (request.LogoFile != null)
                logo = await FileUpload(request.LogoFile);
            string image = "";
            if (request.ImageFile != null)
                image = await FileUpload(request.ImageFile);

            var newCat = new Service.Business
            {
                CoName = request.CoName,
                Tel = request.Tel,
                Email = request.Email,
                Website = request.Website,
                BusinessType = request.BusinessType,
                CountryId = request.CountryId,
                ProvinceId = request.ProvinceId,
                CityId = request.CityId,
                Address = request.Address,
                Location = request.Location,
                Logo = logo,
                Image = image,
                Description = request.Description,
                IndustrialParkId = request.IndustrialParkId == 0 ? null : request.IndustrialParkId,
                CatgoryId = request.CatgoryId,
                CreatorId = theUser.Id,
                Status = BusinessStatus.RegisterRequest
            };

            iITCFServ.IBusinessServ.Add(newCat);
            await iITCFServ.IBusinessServ.SaveChangesAsync();
            return Redirect("/Panel/BusinessManage");

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

            var item = iITCFServ.IBusinessServ.Find(x => x.Id == Id);
            var request = new BusinessRequest
            {
                Id = item.Id,
                CoName = item.CoName,
                Tel = item.Tel,
                Email = item.Email,
                Website = item.Website,
                BusinessType = item.BusinessType,
                CountryId = item.CountryId,
                ProvinceId = item.ProvinceId,
                CityId = item.CityId,
                Address = item.Address,
                Location = item.Location,
                Logo = item.Logo,
                Image = item.Image,
                Description = item.Description,
                IndustrialParkId = item.IndustrialParkId,
                CatgoryId = item.CatgoryId
            };
            DropDownBinder(request);
            return View(GetViewName(lang, "Edit"), request);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromQuery] string lang, [FromForm] BusinessRequest request)
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

            string logo = "";
            if (request.LogoFile != null)
                logo = await FileUpload(request.LogoFile);
            string image = "";
            if (request.ImageFile != null)
                image = await FileUpload(request.ImageFile);



            var theContent = iITCFServ.IBusinessServ.Find(x => x.Id == request.Id);
            theContent.CoName = request.CoName;
            theContent.Tel = request.Tel;
            theContent.Email = request.Email;
            theContent.Website = request.Website;
            theContent.BusinessType = request.BusinessType;
            theContent.CountryId = request.CountryId;
            theContent.ProvinceId = request.ProvinceId;
            theContent.CityId = request.CityId;
            theContent.Address = request.Address;
            theContent.Location = request.Location;
            if (!string.IsNullOrEmpty(logo))
                theContent.Logo = logo;
            if (!string.IsNullOrEmpty(image))
                theContent.Image = image;
            theContent.Description = request.Description;
            theContent.IndustrialParkId = request.IndustrialParkId == 0 ? null : request.IndustrialParkId;
            theContent.Status = theContent.Status == BusinessStatus.EditConfirm ? BusinessStatus.RegisterConfirm : theContent.Status;
            await iITCFServ.IBusinessServ.SaveChangesAsync();

            return Redirect("/Panel/BusinessManage");
        }


        public async Task<IActionResult> Remove(int Id)
        {
            var theContent = iITCFServ.IBusinessServ.Find(x => x.Id == Id);
            iITCFServ.IBusinessServ.Remove(theContent);
            await iITCFServ.IBusinessServ.SaveChangesAsync();
            return Redirect("/Panel/BusinessManage");
        }

        public async Task<IActionResult> Enable(int Id)
        {
            var theContent = iITCFServ.IBusinessCategoryServ.Find(x => x.Id == Id);
            //theContent.Enabled = !theContent.Enabled;
            await iITCFServ.IBusinessServ.SaveChangesAsync();
            return Redirect("/Panel/BusinessManage");
        }

        public async Task<IActionResult> EditRequest(int Id)
        {
            var theContent = iITCFServ.IBusinessServ.Find(x => x.Id == Id);
            theContent.Status = BusinessStatus.EditRequest;
            await iITCFServ.IBusinessServ.SaveChangesAsync();
            return Redirect("/Panel/BusinessManage");
        }

        public async Task<IActionResult> ShowRequest(int Id)
        {
            var theContent = iITCFServ.IBusinessServ.Find(x => x.Id == Id);
            theContent.Status = BusinessStatus.ShowRequest;
            await iITCFServ.IBusinessServ.SaveChangesAsync();
            return Redirect("/Panel/BusinessManage");
        }

        private void DropDownBinder(BusinessRequest request)
        {
            var countries = iITCFServ.iCountryServ.GetAll(x => true);
            ViewBag.Country = new SelectList(countries, "Id", "Title", request?.CountryId);

            var IndustrialParks = iITCFServ.IIndustrialParkServ.GetAll(x => true);
            ViewBag.Parks = new SelectList(IndustrialParks, "Id", "Title", request?.IndustrialParkId);

            var categories = iITCFServ.IBusinessCategoryServ.GetAll(x => true);
            ViewBag.Categories = new SelectList(categories, "Id", "Title", request?.CatgoryId);
        }

        private bool FormVlide(string lang, BusinessRequest request)
        {
            bool result = true;
            if (string.IsNullOrEmpty(request.CoName))
            {
                if (lang == "fa")
                    AddError("نام شرکت/تولیدی باید مقدار داشته باشد", "fa");
                else
                    AddError("Company name can not be null", "en");
                result = false;
            }

            if (string.IsNullOrEmpty(request.Tel))
            {
                if (lang == "fa")
                    AddError("شماره تماس باید مقدار داشته باشد", "fa");
                else
                    AddError("Company Tel can not be null", "en");
                result = false;
            }

            if (request.CountryId == 0)
            {
                if (lang == "fa")
                    AddError("کشور باید مقدار داشته باشد", "fa");
                else
                    AddError("Country can not be null", "en");
                result = false;
            }

            if (request.CountryId == 1 && (request.ProvinceId == null || request.ProvinceId == 0))
            {
                if (lang == "fa")
                    AddError("استان باید مقدار داشته باشد", "fa");
                else
                    AddError("Province can not be null", "en");
                result = false;
            }

            if (request.CityId == 0)
            {
                if (lang == "fa")
                    AddError("شهر باید مقدار داشته باشد", "fa");
                else
                    AddError("City can not be null", "en");
                result = false;
            }

            if (string.IsNullOrEmpty(request.Address))
            {
                if (lang == "fa")
                    AddError("آدرس باید مقدار داشته باشد", "fa");
                else
                    AddError("Address can not be null", "en");
                result = false;
            }

            if (request.CatgoryId == 0)
            {
                if (lang == "fa")
                    AddError("دسته بندی باید مقدار داشته باشد", "fa");
                else
                    AddError("Category can not be null", "en");
                result = false;
            }

            if (request.Id == 0 && request.LogoFile.Length == 0)
            {
                if (lang == "fa")
                    AddError("لوگو باید مقدار داشته باشد", "fa");
                else
                    AddError("Logo can not be null", "en");
                result = false;
            }

            if (request.Id == 0 && request.ImageFile.Length == 0)
            {
                if (lang == "fa")
                    AddError("تصویر باید مقدار داشته باشد", "fa");
                else
                    AddError("تصویر can not be null", "en");
                result = false;
            }

            return result;
        }


        private async Task<string> FileUpload(IFormFile file)
        {
            string Image = string.Empty;
            if (file != null && file.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = file,
                    RootPath = hosting.ContentRootPath,
                    UnitPath = Config.GetSection("FileRoot:BusinessFile").Value
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
                }

                Image = SaveImage.FilePath;
            }

            return Image;
        }

    }
}
