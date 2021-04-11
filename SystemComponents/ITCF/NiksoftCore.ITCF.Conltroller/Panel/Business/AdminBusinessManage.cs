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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.ITCF.Conltroller.Panel.Business
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin,Admin")]
    public class AdminBusinessManage : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        public IITCFService iITCFServ { get; set; }
        private readonly IWebHostEnvironment hosting;

        public AdminBusinessManage(IConfiguration Configuration, IWebHostEnvironment hostingEnvironment,
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

            var total = iITCFServ.IBusinessServ.Count(x => true);
            var pager = new Pagination(total, 20, part);
            ViewBag.Pager = pager;

            if (lang == "fa")
                ViewBag.PageTitle = "مدیریت کسب و کار";
            else
                ViewBag.PageTitle = "Business Management";

            ViewBag.Contents = iITCFServ.IBusinessServ.GetPart(x => true ||
            x.Status == BusinessStatus.EditRequest
            , pager.StartIndex, pager.PageSize).ToList();

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
                IndustrialParkId = request.IndustrialParkId,
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
            theContent.IndustrialParkId = request.IndustrialParkId;
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

        public async Task<IActionResult> Confirm(int Id)
        {
            var theContent = iITCFServ.IBusinessServ.Find(x => x.Id == Id);
            var theUser = userManager.Users.Where(x => x.Id == theContent.CreatorId).First();
            var hasRole = await userManager.IsInRoleAsync(theUser, "Business");
            if (theContent.Status == BusinessStatus.RegisterRequest)
            {
                theContent.Status = BusinessStatus.RegisterConfirm;
                if (!hasRole)
                {
                    await userManager.AddToRoleAsync(theUser, "Business");
                }
            }
            else if (theContent.Status == BusinessStatus.EditRequest)
            {
                theContent.Status = BusinessStatus.EditConfirm;
            }
            else if (theContent.Status == BusinessStatus.ShowRequest)
            {
                theContent.Status = BusinessStatus.ConfirmShow;
            }

            await iITCFServ.IBusinessServ.SaveChangesAsync();
            return Redirect("/Panel/AdminBusinessManage");
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

            return result;
        }



        [HttpGet]
        public async Task<IActionResult> Products(int part, int bid)
        {
            //var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = await iITCFServ.IBusinessServ.FindAsync(x => x.Id == bid);
            if (theBusiness == null)
            {
                return Redirect("/Panel");
            }

            ViewBag.Company = theBusiness;

            ViewBag.PageTitle = "محصولات";

            var total = iITCFServ.iProductServ.Count(x => x.BusinessId == bid);
            var pager = new Pagination(total, 20, part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iITCFServ.iProductServ.GetAll(x => x.BusinessId == bid).ToList();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct([FromQuery] string lang, int bid)
        {
            //var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = await iITCFServ.IBusinessServ.FindAsync(x => x.Id == bid);
            if (theBusiness == null)
            {
                return Redirect("/Panel");
            }
            ViewBag.Company = theBusiness;

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (lang == "fa")
                ViewBag.PageTitle = "ایجاد محصول";
            else
                ViewBag.PageTitle = "Create Product";

            var request = new ProductRequest();
            request.BusinessId = bid;
            request.BusinessCatId = theBusiness.CatgoryId;
            DropDownBinder(request);
            return View(GetViewName(lang, "CreateProduct"), request);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromQuery] string lang, ProductRequest request)
        {
            //var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = await iITCFServ.IBusinessServ.FindAsync(x => x.Id == request.BusinessId);
            if (theBusiness == null)
            {
                return Redirect("/Panel");
            }
            ViewBag.Company = theBusiness;

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (!FormProduct(lang, request))
            {
                ViewBag.Messages = Messages;
                request.BusinessCatId = theBusiness.CatgoryId;
                DropDownBinder(request);
                return View(GetViewName(lang, "CreateProduct"), request);
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
                    Messages.Add(new NikMessage
                    {
                        Message = "آپلود فایل انجام نشد مجدد تلاش کنید",
                        Type = MessageType.Error,
                        Language = "Fa"
                    });
                    ViewBag.Messages = Messages;
                    DropDownBinder(request);
                    return View(GetViewName(lang, "CreateProduct"), request);
                }

                Image = SaveImage.FilePath;
            }

            string Video = string.Empty;
            if (request.VideoFile != null && request.VideoFile.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.VideoFile,
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
                    DropDownBinder(request);
                    return View(GetViewName(lang, "CreateProduct"), request);
                }

                Video = SaveImage.FilePath;
            }

            var newItem = new Product
            {
                Title = request.Title,
                EnTitle = request.EnTitle,
                ArTitle = request.ArTitle,
                Description = request.Description,
                EnDescription = request.EnDescription,
                ArDescription = request.ArDescription,
                Image = Image,
                Video = Video,
                Price = request.Price,
                CategoryId = request.CategoryId,
                BusinessId = request.BusinessId
            };

            iITCFServ.iProductServ.Add(newItem);
            await iITCFServ.iProductServ.SaveChangesAsync();
            return Redirect("/Panel/AdminBusinessManage/Products/?bid=" + request.BusinessId);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct([FromQuery] string lang, int Id)
        {
            //var theUser = await userManager.GetUserAsync(HttpContext.User);
            var item = iITCFServ.iProductServ.Find(x => x.Id == Id);
            var theBusiness = await iITCFServ.IBusinessServ.FindAsync(x => x.Id == item.BusinessId);
            if (theBusiness == null)
            {
                return Redirect("/Panel");
            }
            ViewBag.Company = theBusiness;

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (lang == "fa")
                ViewBag.PageTitle = "بروزرسانی محصول";
            else
                ViewBag.PageTitle = "Update Product";

            var request = new ProductRequest();
            request.Title = item.Title;
            request.EnTitle = item.EnTitle;
            request.ArTitle = item.ArTitle;
            request.Description = item.Description;
            request.EnDescription = item.EnDescription;
            request.ArDescription = item.ArDescription;
            request.Image = item.Image;
            request.Video = item.Video;
            request.Price = item.Price;
            request.CategoryId = item.CategoryId;
            request.BusinessId = item.BusinessId;
            request.BusinessCatId = theBusiness.CatgoryId;
            DropDownBinder(request);
            return View(GetViewName(lang, "EditProduct"), request);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct([FromQuery] string lang, ProductRequest request)
        {
            //var theUser = await userManager.GetUserAsync(HttpContext.User);
            var item = await iITCFServ.iProductServ.FindAsync(x => x.Id == request.Id);
            var theBusiness = await iITCFServ.IBusinessServ.FindAsync(x => x.Id == request.BusinessId);
            request.BusinessCatId = theBusiness.CatgoryId;
            if (theBusiness == null)
            {
                return Redirect("/Panel");
            }
            ViewBag.Company = theBusiness;

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (!FormProduct(lang, request))
            {
                ViewBag.Messages = Messages;
                DropDownBinder(request);
                return View(GetViewName(lang, "EditProduct"), request);
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
                    Messages.Add(new NikMessage
                    {
                        Message = "آپلود فایل انجام نشد مجدد تلاش کنید",
                        Type = MessageType.Error,
                        Language = "Fa"
                    });
                    ViewBag.Messages = Messages;
                    DropDownBinder(request);
                    return View(GetViewName(lang, "CreateGroup"), request);
                }

                Image = SaveImage.FilePath;
            }

            string Video = string.Empty;
            if (request.VideoFile != null && request.VideoFile.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.VideoFile,
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
                    DropDownBinder(request);
                    return View(GetViewName(lang, "EditProduct"), request);
                }

                Video = SaveImage.FilePath;
            }

            item.CategoryId = request.CategoryId;
            item.Title = request.Title;
            item.EnTitle = request.EnTitle;
            item.ArTitle = request.ArTitle;
            item.Description = request.Description;
            item.EnDescription = request.EnDescription;
            item.ArDescription = request.ArDescription;
            if (!string.IsNullOrEmpty(Image))
                item.Image = Image;
            if (!string.IsNullOrEmpty(Video))
                item.Video = Video;
            item.Price = request.Price;
            await iITCFServ.iProductServ.SaveChangesAsync();
            return Redirect("/Panel/AdminBusinessManage/Products/?bid=" + request.BusinessId);
        }

        public async Task<IActionResult> RemoveProduct(int Id)
        {
            var theContent = iITCFServ.iProductServ.Find(x => x.Id == Id);
            int bid = theContent.BusinessId;
            if (!string.IsNullOrEmpty(theContent.Image))
            {
                NikTools.RemoveFile(new RemoveFileRequest
                {
                    RootPath = hosting.ContentRootPath,
                    FilePath = theContent.Image
                });
            }

            if (!string.IsNullOrEmpty(theContent.Video))
            {
                NikTools.RemoveFile(new RemoveFileRequest
                {
                    RootPath = hosting.ContentRootPath,
                    FilePath = theContent.Video
                });
            }

            iITCFServ.iProductServ.Remove(theContent);
            await iITCFServ.iProductServ.SaveChangesAsync();
            return Redirect("/Panel/AdminBusinessManage/Products/?bid=" + bid);
        }

        public async Task<IActionResult> ProductFiles(int Id, int part)
        {
            var theProduct = await iITCFServ.iProductServ.FindAsync(x => x.Id == Id);
            if (theProduct == null)
            {
                return Redirect("/Panel");
            }

            ViewBag.Product = theProduct;

            ViewBag.PageTitle = "فایل های " + theProduct.Title;

            var total = iITCFServ.iProductFileServ.Count(x => x.ProductId == Id);
            var pager = new Pagination(total, 20, part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iITCFServ.iProductFileServ.GetAll(x => x.ProductId == Id).ToList();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateFile(int Id, int ProductId)
        {
            var theProduct = await iITCFServ.iProductServ.FindAsync(x => x.Id == ProductId);
            if (theProduct == null)
            {
                return Redirect("/Panel");
            }
            ViewBag.Product = theProduct;

            ViewBag.PageTitle = "افزودن تصاویر محصول " + theProduct.Title;

            var request = new ProductFileRequest();

            if (Id > 0)
            {
                var thisItem = await iITCFServ.iProductFileServ.FindAsync(x => x.Id == Id);
                request.Id = thisItem.Id;
                request.Title = thisItem.Title;
                request.EnTitle = thisItem.EnTitle;
                request.ArTitle = thisItem.ArTitle;
                request.Description = thisItem.Description;
                request.EnDescription = thisItem.EnDescription;
                request.ArDescription = thisItem.ArDescription;
                request.Path = thisItem.Path;
                request.ProductId = thisItem.ProductId;
            }
            else
            {
                request.ProductId = ProductId;
            }

            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFile(ProductFileRequest request)
        {
            var theProduct = await iITCFServ.iProductServ.FindAsync(x => x.Id == request.ProductId);
            if (theProduct == null)
            {
                return Redirect("/Panel");
            }
            ViewBag.Product = theProduct;

            if (!FormFileCheck(request))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            string Image = string.Empty;
            if (request.FileData != null && request.FileData.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.FileData,
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
                    return View(request);
                }

                Image = SaveImage.FilePath;
            }

            ProductFile item = new ProductFile();

            if (request.Id > 0)
            {
                item = await iITCFServ.iProductFileServ.FindAsync(x => x.Id == request.Id);
            }

            item.Title = request.Title;
            item.EnTitle = request.EnTitle;
            item.ArTitle = request.ArTitle;
            item.Description = request.Description;
            item.EnDescription = request.EnDescription;
            item.ArDescription = request.ArDescription;
            item.Path = Image;
            item.ProductId = request.ProductId;

            if (request.Id == 0)
            {
                iITCFServ.iProductFileServ.Add(item);
            }
            
            await iITCFServ.iProductFileServ.SaveChangesAsync();
            return Redirect("/Panel/AdminBusinessManage/ProductFiles/?Id=" + request.ProductId);
        }

        public async Task<IActionResult> RemoveFile(int Id)
        {
            var theContent = iITCFServ.iProductFileServ.Find(x => x.Id == Id);
            if (!string.IsNullOrEmpty(theContent.Path))
            {
                NikTools.RemoveFile(new RemoveFileRequest
                {
                    RootPath = hosting.ContentRootPath,
                    FilePath = theContent.Path
                });
            }

            iITCFServ.iProductFileServ.Remove(theContent);
            await iITCFServ.iProductServ.SaveChangesAsync();
            return Redirect("/Panel/AdminBusinessManage/ProductFiles/?Id=" + theContent.ProductId);
        }

        private void DropDownBinder(ProductRequest request)
        {
            var cats = new List<BusinessCategory>();
            var categories = iITCFServ.IBusinessCategoryServ.GetAll(x => x.ParentId == request.BusinessCatId);
            foreach (var item in categories)
            {
                cats.Add(item);
                foreach (var subs in item.Childs)
                {
                    cats.Add(item);
                }
            }

            ViewBag.Categories = new SelectList(cats, "Id", "Title", request?.CategoryId);
        }

        private bool FormProduct(string lang, ProductRequest request)
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

            if (string.IsNullOrEmpty(request.Description))
            {
                if (lang == "fa")
                    AddError("توضیحات باید مقدار داشته باشد", "fa");
                else
                    AddError("Description can not be null", "en");
                result = false;
            }

            if (request.CategoryId == 0)
            {
                if (lang == "fa")
                    AddError("دسته بندی باید مقدار داشته باشد", "fa");
                else
                    AddError("Description can not be null", "en");
                result = false;
            }

            if (request.Id == 0 && request.ImageFile == null)
            {
                if (lang == "fa")
                    AddError("تصویر نمی تواند خالی باشد", "fa");
                else
                    AddError("Title can not be null", "en");
                result = false;
            }

            if (request.ImageFile != null && request.ImageFile.Length > 512000)
            {
                if (lang == "fa")
                    AddError("حجم تصویر نباید بیشتر از 500 KB باشد", "fa");
                else
                    AddError("Title can not be null", "en");
                result = false;
            }

            if (request.VideoFile != null && request.VideoFile.FileName.GetExtention() != "mp4")
            {
                if (lang == "fa")
                    AddError("فرمت فایل صحیح نیست", "fa");
                else
                    AddError("Title can not be null", "en");
                result = false;
            }

            if (request.VideoFile != null && request.VideoFile.Length > 2242880)
            {
                if (lang == "fa")
                    AddError("حجم ویدیو نباید بیشتر از 5 MB باشد", "fa");
                else
                    AddError("Title can not be null", "en");
                result = false;
            }

            return result;
        }

        private bool FormFileCheck(ProductFileRequest request)
        {
            bool result = true;
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("عنوان فارسی باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (request.FileData == null || request.FileData.Length == 0)
            {
                AddError("هیچ فایلی انتخاب نشده است", "fa");
                result = false;
            }

            if (request.FileData != null && request.FileData.Length > 512000)
            {
                AddError("حجم تصویر نباید بیشتر از 500 KB باشد", "fa");
                result = false;
            }

            return result;
        }




    }
}
