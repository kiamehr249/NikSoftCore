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
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.ITCF.Conltroller.Panel.Business
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin,Admin,Business")]
    public class CompanyDataManage : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        public IITCFService iITCFServ { get; set; }
        private readonly IWebHostEnvironment hosting;

        public CompanyDataManage(IConfiguration Configuration, IWebHostEnvironment hostingEnvironment,
            UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            iITCFServ = new ITCFService(Configuration);
            hosting = hostingEnvironment;
        }

        public async Task<IActionResult> Index([FromQuery] string lang, int bid)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == bid && x.CreatorId == theUser.Id);
            if (theBusiness == null)
            {
                return Redirect("/Panel");
            }

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            ViewBag.Company = theBusiness;

            if (lang == "fa")
                ViewBag.PageTitle = "مدیریت محتوای کسب و کار";
            else
                ViewBag.PageTitle = "Company Content Management";

            return View(GetViewName(lang, "Index"));
        }


        public async Task<IActionResult> IntroHome([FromQuery] string lang, int bid)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == bid && x.CreatorId == theUser.Id);
            if (theBusiness == null)
            {
                return Redirect("/Panel");
            }

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            ViewBag.Company = theBusiness;

            if (lang == "fa")
                ViewBag.PageTitle = "مدیریت محتوای کسب و کار";
            else
                ViewBag.PageTitle = "Company Content Management";

            return View(GetViewName(lang, "IntroHome"));
        }


        public async Task<IActionResult> IntroList([FromQuery] string lang, int bid, int part, int unit)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == bid && x.CreatorId == theUser.Id);
            if (theBusiness == null)
            {
                return Redirect("/Panel");
            }

            ViewBag.Company = theBusiness;
            ViewBag.Create = "CreatePart" + unit;
            ViewBag.Unit = unit;

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();



            var keyValue = Tools.GetUnitKey(unit);
            var total = iITCFServ.iIntroductionServ.Count(x => x.BusinessId == bid && x.UserId == theUser.Id && x.KeyValue == keyValue);
            var pager = new Pagination(total, 20, part);
            ViewBag.Pager = pager;

            if (lang == "fa")
                ViewBag.PageTitle = Tools.GetUnitTitle(unit);
            else
                ViewBag.PageTitle = "Company Content Management";

            ViewBag.Contents = iITCFServ.iIntroductionServ.GetPart(x => x.BusinessId == bid && x.UserId == theUser.Id && x.KeyValue == keyValue, pager.StartIndex, pager.PageSize).ToList();

            return View(GetViewName(lang, "IntroList"), new { lang, bid, part, unit });
        }

        [HttpGet]
        public async Task<IActionResult> CreatePart1([FromQuery] string lang, int bid)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == bid && x.CreatorId == theUser.Id);
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
                ViewBag.PageTitle = Tools.GetUnitTitle(1);
            else
                ViewBag.PageTitle = "Company Content Management";



            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            var request = new IntroductionRequest();
            request.BusinessId = theBusiness.Id;

            return View(GetViewName(lang, "CreatePart1"), request);

        }

        [HttpPost]
        public async Task<IActionResult> CreatePart1([FromQuery] string lang, [FromForm] IntroductionRequest request)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == request.BusinessId && x.CreatorId == theUser.Id);
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
                ViewBag.PageTitle = Tools.GetUnitTitle(1);
            else
                ViewBag.PageTitle = "Company Content Management";

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (!FormVlideUnit1(lang, request))
            {
                ViewBag.Messages = Messages;
                return View(GetViewName(lang, "CreatePart1"), request);
            }

            string Image = string.Empty;
            if (request.Image != null && request.Image.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.Image,
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
                    return View(GetViewName(lang, "CreatePart1"), request);
                }

                Image = SaveImage.FilePath;
            }

            var newItem = new Introduction
            {
                Title = request.Title,
                Description = request.Description,
                ImageFile = Image,
                KeyValue = Tools.GetUnitKey(1),
                BusinessId = theBusiness.Id,
                UserId = theUser.Id,
                CreateDate = DateTime.Now
            };

            iITCFServ.iIntroductionServ.Add(newItem);
            await iITCFServ.iIntroductionServ.SaveChangesAsync();
            return Redirect("/Panel/CompanyDataManage/IntroList/?bid=" + request.BusinessId + "&unit=1");

        }

        [HttpGet]
        public async Task<IActionResult> CreatePart2([FromQuery] string lang, int bid)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == bid && x.CreatorId == theUser.Id);
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
                ViewBag.PageTitle = Tools.GetUnitTitle(2);
            else
                ViewBag.PageTitle = "Company Content Management";

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            var request = new IntroductionRequest();
            request.BusinessId = theBusiness.Id;

            return View(GetViewName(lang, "CreatePart2"), request);

        }

        [HttpPost]
        public async Task<IActionResult> CreatePart2([FromQuery] string lang, [FromForm] IntroductionRequest request)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == request.BusinessId && x.CreatorId == theUser.Id);
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
                ViewBag.PageTitle = Tools.GetUnitTitle(2);
            else
                ViewBag.PageTitle = "Company Content Management";

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (!FormVlideUnit1(lang, request))
            {
                ViewBag.Messages = Messages;
                return View(GetViewName(lang, "CreatePart2"), request);
            }

            string Image = string.Empty;
            if (request.Image != null && request.Image.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.Image,
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
                    return View(GetViewName(lang, "CreatePart2"), request);
                }

                Image = SaveImage.FilePath;
            }

            var newItem = new Introduction
            {
                Title = request.Title,
                Description = request.Description,
                ImageFile = Image,
                KeyValue = Tools.GetUnitKey(2),
                BusinessId = theBusiness.Id,
                UserId = theUser.Id,
                CreateDate = DateTime.Now
            };

            iITCFServ.iIntroductionServ.Add(newItem);
            await iITCFServ.iIntroductionServ.SaveChangesAsync();
            return Redirect("/Panel/CompanyDataManage/IntroList/?bid=" + request.BusinessId + "&unit=2");

        }

        [HttpGet]
        public async Task<IActionResult> CreatePart3([FromQuery] string lang, int bid)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == bid && x.CreatorId == theUser.Id);
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
                ViewBag.PageTitle = Tools.GetUnitTitle(3);
            else
                ViewBag.PageTitle = "Company Content Management";

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            var request = new IntroductionRequest();
            request.BusinessId = theBusiness.Id;

            return View(GetViewName(lang, "CreatePart3"), request);

        }

        [HttpPost]
        public async Task<IActionResult> CreatePart3([FromQuery] string lang, [FromForm] IntroductionRequest request)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == request.BusinessId && x.CreatorId == theUser.Id);
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
                ViewBag.PageTitle = Tools.GetUnitTitle(3);
            else
                ViewBag.PageTitle = "Company Content Management";

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (!FormVlideUnit3(lang, request))
            {
                ViewBag.Messages = Messages;
                return View(GetViewName(lang, "CreatePart3"), request);
            }

            string image = string.Empty;
            if (request.Image != null && request.Image.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.Image,
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
                    return View(GetViewName(lang, "CreatePart3"), request);
                }

                image = SaveImage.FilePath;
            }

            string video = string.Empty;
            if (request.Video != null && request.Video.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.Video,
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
                    return View(GetViewName(lang, "CreatePart3"), request);
                }

                video = SaveImage.FilePath;
            }

            var newItem = new Introduction
            {
                Title = request.Title,
                Description = request.Description,
                ImageFile = image,
                VideoFile = video,
                KeyValue = Tools.GetUnitKey(3),
                BusinessId = theBusiness.Id,
                UserId = theUser.Id,
                CreateDate = DateTime.Now
            };

            iITCFServ.iIntroductionServ.Add(newItem);
            await iITCFServ.iIntroductionServ.SaveChangesAsync();
            return Redirect("/Panel/CompanyDataManage/IntroList/?bid=" + request.BusinessId + "&unit=3");

        }

        [HttpGet]
        public async Task<IActionResult> EditPart1([FromQuery] string lang, int Id)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var item = iITCFServ.iIntroductionServ.Find(x => x.Id == Id);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == item.BusinessId && x.CreatorId == theUser.Id);
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
                ViewBag.PageTitle = Tools.GetUnitTitle(1);
            else
                ViewBag.PageTitle = "Company Content Management";



            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            var request = new IntroductionRequest
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                ImageFile = item.ImageFile,
                VideoFile = item.VideoFile,
                SoundFile = item.SoundFile,
                KeyValue = item.KeyValue,
                BusinessId = item.BusinessId,
                UserId = item.UserId,
                CreateDate = item.CreateDate
            };

            return View(GetViewName(lang, "EditPart1"), request);

        }

        [HttpPost]
        public async Task<IActionResult> EditPart1([FromQuery] string lang, [FromForm] IntroductionRequest request)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var item = iITCFServ.iIntroductionServ.Find(x => x.Id == request.Id);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == request.BusinessId && x.CreatorId == theUser.Id);
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
                ViewBag.PageTitle = Tools.GetUnitTitle(1);
            else
                ViewBag.PageTitle = "Company Content Management";

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();
            if (item.KeyValue == "unit1" || item.KeyValue == "unit2")
            {
                if (!FormVlideUnit1(lang, request))
                {
                    ViewBag.Messages = Messages;
                    return View(GetViewName(lang, "EditPart1"), request);
                }

                string Image = string.Empty;
                if (request.Image != null && request.Image.Length > 0)
                {
                    var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                    {
                        File = request.Image,
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
                        return View(GetViewName(lang, "EditPart1"), request);
                    }

                    Image = SaveImage.FilePath;
                }

                if (!string.IsNullOrEmpty(Image))
                    item.ImageFile = Image;

            }
            else
            {
                if (!FormVlideUnit3(lang, request))
                {
                    ViewBag.Messages = Messages;
                    return View(GetViewName(lang, "EditPart1"), request);
                }

                string video = string.Empty;
                if (request.Video != null && request.Video.Length > 0)
                {
                    var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                    {
                        File = request.Video,
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
                        return View(GetViewName(lang, "EditPart1"), request);
                    }

                    video = SaveImage.FilePath;
                }

                if (!string.IsNullOrEmpty(video))
                    item.VideoFile = video;
            }




            item.Title = request.Title;
            item.Description = request.Description;

            await iITCFServ.iIntroductionServ.SaveChangesAsync();
            return Redirect("/Panel/CompanyDataManage/IntroList/?bid=" + request.BusinessId + "&unit=" + Tools.GetUnitNum(item.KeyValue));

        }

        [HttpGet]
        public async Task<IActionResult> EditPart2([FromQuery] string lang, int Id)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var item = iITCFServ.iIntroductionServ.Find(x => x.Id == Id);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == item.BusinessId && x.CreatorId == theUser.Id);
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
                ViewBag.PageTitle = Tools.GetUnitTitle(1);
            else
                ViewBag.PageTitle = "Company Content Management";



            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            var request = new IntroductionRequest
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                ImageFile = item.ImageFile,
                VideoFile = item.VideoFile,
                SoundFile = item.SoundFile,
                KeyValue = item.KeyValue,
                BusinessId = item.BusinessId,
                UserId = item.UserId,
                CreateDate = item.CreateDate
            };

            return View(GetViewName(lang, "EditPart2"), request);

        }

        [HttpPost]
        public async Task<IActionResult> EditPart2([FromQuery] string lang, [FromForm] IntroductionRequest request)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var item = iITCFServ.iIntroductionServ.Find(x => x.Id == request.Id);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == request.BusinessId && x.CreatorId == theUser.Id);
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
                ViewBag.PageTitle = Tools.GetUnitTitle(1);
            else
                ViewBag.PageTitle = "Company Content Management";

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();
            if (item.KeyValue == "unit1" || item.KeyValue == "unit2")
            {
                if (!FormVlideUnit1(lang, request))
                {
                    ViewBag.Messages = Messages;
                    return View(GetViewName(lang, "EditPart2"), request);
                }

                string Image = string.Empty;
                if (request.Image != null && request.Image.Length > 0)
                {
                    var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                    {
                        File = request.Image,
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
                        return View(GetViewName(lang, "EditPart2"), request);
                    }

                    Image = SaveImage.FilePath;
                }

                if (!string.IsNullOrEmpty(Image))
                    item.ImageFile = Image;

            }
            else
            {
                if (!FormVlideUnit3(lang, request))
                {
                    ViewBag.Messages = Messages;
                    return View(GetViewName(lang, "EditPart2"), request);
                }

                string video = string.Empty;
                if (request.Video != null && request.Video.Length > 0)
                {
                    var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                    {
                        File = request.Video,
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
                        return View(GetViewName(lang, "EditPart2"), request);
                    }

                    video = SaveImage.FilePath;
                }

                if (!string.IsNullOrEmpty(video))
                    item.VideoFile = video;
            }




            item.Title = request.Title;
            item.Description = request.Description;

            await iITCFServ.iIntroductionServ.SaveChangesAsync();
            return Redirect("/Panel/CompanyDataManage/IntroList/?bid=" + request.BusinessId + "&unit=" + Tools.GetUnitNum(item.KeyValue));

        }

        [HttpGet]
        public async Task<IActionResult> EditPart3([FromQuery] string lang, int Id)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var item = iITCFServ.iIntroductionServ.Find(x => x.Id == Id);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == item.BusinessId && x.CreatorId == theUser.Id);
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
                ViewBag.PageTitle = Tools.GetUnitTitle(1);
            else
                ViewBag.PageTitle = "Company Content Management";



            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            var request = new IntroductionRequest
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                ImageFile = item.ImageFile,
                VideoFile = item.VideoFile,
                SoundFile = item.SoundFile,
                KeyValue = item.KeyValue,
                BusinessId = item.BusinessId,
                UserId = item.UserId,
                CreateDate = item.CreateDate
            };

            return View(GetViewName(lang, "EditPart3"), request);

        }

        [HttpPost]
        public async Task<IActionResult> EditPart3([FromQuery] string lang, [FromForm] IntroductionRequest request)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var item = iITCFServ.iIntroductionServ.Find(x => x.Id == request.Id);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == request.BusinessId && x.CreatorId == theUser.Id);
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
                ViewBag.PageTitle = Tools.GetUnitTitle(1);
            else
                ViewBag.PageTitle = "Company Content Management";

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();


            if (!FormVlideUnit3(lang, request))
            {
                ViewBag.Messages = Messages;
                return View(GetViewName(lang, "EditPart3"), request);
            }

            string image = string.Empty;
            if (request.Image != null && request.Image.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.Image,
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
                    return View(GetViewName(lang, "EditPart3"), request);
                }

                image = SaveImage.FilePath;
            }

            if (!string.IsNullOrEmpty(image))
                item.ImageFile = image;

            string video = string.Empty;
            if (request.Video != null && request.Video.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.Video,
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
                    return View(GetViewName(lang, "EditPart3"), request);
                }

                video = SaveImage.FilePath;
            }

            if (!string.IsNullOrEmpty(video))
                item.VideoFile = video;




            item.Title = request.Title;
            item.Description = request.Description;

            await iITCFServ.iIntroductionServ.SaveChangesAsync();
            return Redirect("/Panel/CompanyDataManage/IntroList/?bid=" + request.BusinessId + "&unit=" + Tools.GetUnitNum(item.KeyValue));

        }

        public async Task<IActionResult> Remove(int Id, int bid, int unit)
        {
            var theContent = iITCFServ.iIntroductionServ.Find(x => x.Id == Id);
            if (!string.IsNullOrEmpty(theContent.ImageFile))
            {
                NikTools.RemoveFile(new RemoveFileRequest
                {
                    RootPath = hosting.ContentRootPath,
                    FilePath = theContent.ImageFile
                });
            }

            if (!string.IsNullOrEmpty(theContent.VideoFile))
            {
                NikTools.RemoveFile(new RemoveFileRequest
                {
                    RootPath = hosting.ContentRootPath,
                    FilePath = theContent.VideoFile
                });
            }

            if (!string.IsNullOrEmpty(theContent.SoundFile))
            {
                NikTools.RemoveFile(new RemoveFileRequest
                {
                    RootPath = hosting.ContentRootPath,
                    FilePath = theContent.SoundFile
                });
            }

            iITCFServ.iIntroductionServ.Remove(theContent);
            await iITCFServ.iIntroductionServ.SaveChangesAsync();
            return Redirect("/Panel/CompanyDataManage/IntroList/?bid=" + bid + "&unit=" + unit);
        }

        private bool FormVlideUnit1(string lang, IntroductionRequest request)
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
                    AddError("Title can not be null", "en");
                result = false;
            }

            if (request.Id == 0 && request.Image == null)
            {
                if (lang == "fa")
                    AddError("تصویر باید مقدار داشته باشد", "fa");
                else
                    AddError("Title can not be null", "en");
                result = false;
            }
            else if (request.Id == 0 && request.Image.Length > 512000)
            {
                if (lang == "fa")
                    AddError("حجم تصویر نباید بیشتر از 500 KB باشد", "fa");
                else
                    AddError("Title can not be null", "en");
                result = false;
            }

            return result;
        }

        private bool FormVlideUnit3(string lang, IntroductionRequest request)
        {
            bool result = true;
            if (request.Video == null && request.Id == 0)
            {
                if (lang == "fa")
                    AddError("ویدیو باید مقدار داشته باشد", "fa");
                else
                    AddError("Title can not be null", "en");
                result = false;
            }
            else if (request.Id == 0 && request.Video.FileName.GetExtention() != "mp4")
            {
                if (lang == "fa")
                    AddError("فرمت فایل صحیح نیست", "fa");
                else
                    AddError("Title can not be null", "en");
                result = false;
            }
            else if (request.Id == 0 && request.Video.Length > 2242880)
            {
                if (lang == "fa")
                    AddError("حجم ویدیو نباید بیشتر از 5 MB باشد", "fa");
                else
                    AddError("Title can not be null", "en");
                result = false;
            }



            return result;
        }

        [HttpGet]
        public async Task<IActionResult> ProductCategories([FromQuery] string lang, int part, int bid)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == bid && x.CreatorId == theUser.Id);
            if (theBusiness == null)
            {
                return Redirect("/Panel");
            }

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            ViewBag.Company = theBusiness;

            if (lang == "fa")
                ViewBag.PageTitle = "دسته بندی محصولات";
            else
                ViewBag.PageTitle = "Company Content Management";

            var total = iITCFServ.iProductGroupServ.Count(x => x.BusinessId == bid);
            var pager = new Pagination(total, 20, part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iITCFServ.iProductGroupServ.GetAll(x => x.BusinessId == bid).ToList();

            return View(GetViewName(lang, "ProductCategories"));
        }

        [HttpGet]
        public async Task<IActionResult> CreateGroup([FromQuery] string lang, int bid)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == bid && x.CreatorId == theUser.Id);
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
                ViewBag.PageTitle = "ایجاد دسته بندی";
            else
                ViewBag.PageTitle = "Create Business Category";

            var request = new ProductGroupRequest();
            request.BusinessId = bid;
            return View(GetViewName(lang, "CreateGroup"), request);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromQuery] string lang, ProductGroupRequest request)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == request.BusinessId && x.CreatorId == theUser.Id);
            if (theBusiness == null)
            {
                return Redirect("/Panel");
            }

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (!FormGroup(lang, request))
            {
                ViewBag.Messages = Messages;
                return View(GetViewName(lang, "CreateGroup"), request);
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
                    return View(GetViewName(lang, "CreateGroup"), request);
                }

                Image = SaveImage.FilePath;
            }

            var newItem = new ProductGroup
            {
                Title = request.Title,
                Image = Image,
                BusinessId = request.BusinessId
            };

            iITCFServ.iProductGroupServ.Add(newItem);
            await iITCFServ.iProductGroupServ.SaveChangesAsync();
            return Redirect("/Panel/CompanyDataManage/ProductCategories/?bid=" + request.BusinessId);
        }

        [HttpGet]
        public async Task<IActionResult> EditGroup([FromQuery] string lang, int Id)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var item = iITCFServ.iProductGroupServ.Find(x => x.Id == Id);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == item.BusinessId && x.CreatorId == theUser.Id);
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
                ViewBag.PageTitle = "ایجاد دسته بندی";
            else
                ViewBag.PageTitle = "Create Business Category";

            var request = new ProductGroupRequest();
            request.Title = item.Title;
            request.Image = item.Image;
            request.BusinessId = item.BusinessId;
            return View(GetViewName(lang, "EditGroup"), request);
        }

        [HttpPost]
        public async Task<IActionResult> EditGroup([FromQuery] string lang, ProductGroupRequest request)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var item = iITCFServ.iProductGroupServ.Find(x => x.Id == request.Id);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == request.BusinessId && x.CreatorId == theUser.Id);
            if (theBusiness == null)
            {
                return Redirect("/Panel");
            }

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (!FormGroup(lang, request))
            {
                ViewBag.Messages = Messages;
                return View(GetViewName(lang, "CreateGroup"), request);
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
                    return View(GetViewName(lang, "CreateGroup"), request);
                }

                Image = SaveImage.FilePath;
            }

            item.Title = request.Title;
            if (!string.IsNullOrEmpty(Image))
                item.Image = Image;
            await iITCFServ.iProductGroupServ.SaveChangesAsync();
            return Redirect("/Panel/CompanyDataManage/ProductCategories/?bid=" + request.BusinessId);
        }

        public async Task<IActionResult> RemoveGroup(int Id)
        {
            var theContent = iITCFServ.iProductGroupServ.Find(x => x.Id == Id);
            int bid = theContent.BusinessId;
            if (!string.IsNullOrEmpty(theContent.Image))
            {
                NikTools.RemoveFile(new RemoveFileRequest
                {
                    RootPath = hosting.ContentRootPath,
                    FilePath = theContent.Image
                });
            }

            iITCFServ.iProductGroupServ.Remove(theContent);
            await iITCFServ.iProductGroupServ.SaveChangesAsync();
            return Redirect("/Panel/CompanyDataManage/ProductCategories/?bid=" + bid);
        }
        private bool FormGroup(string lang, ProductGroupRequest request)
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

            if (request.ImageFile != null && request.ImageFile.Length > 512000)
            {
                if (lang == "fa")
                    AddError("حجم تصویر نباید بیشتر از 500 KB باشد", "fa");
                else
                    AddError("Title can not be null", "en");
                result = false;
            }

            return result;
        }


        [HttpGet]
        public async Task<IActionResult> Products([FromQuery] string lang, int part, int bid)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == bid && x.CreatorId == theUser.Id);
            if (theBusiness == null)
            {
                return Redirect("/Panel");
            }

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            ViewBag.Company = theBusiness;

            if (lang == "fa")
                ViewBag.PageTitle = "محصولات";
            else
                ViewBag.PageTitle = "Company Content Management";

            var total = iITCFServ.iProductServ.Count(x => x.BusinessId == bid);
            var pager = new Pagination(total, 20, part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iITCFServ.iProductServ.GetAll(x => x.BusinessId == bid).ToList();

            return View(GetViewName(lang, "Products"));
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct([FromQuery] string lang, int bid)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == bid && x.CreatorId == theUser.Id);
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
            DropDownBinder(request);
            return View(GetViewName(lang, "CreateProduct"), request);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromQuery] string lang, ProductRequest request)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == request.BusinessId && x.CreatorId == theUser.Id);
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
                Description = request.Description,
                Image = Image,
                Video = Video,
                GroupId = request.GroupId,
                BusinessId = request.BusinessId
            };

            iITCFServ.iProductServ.Add(newItem);
            await iITCFServ.iProductServ.SaveChangesAsync();
            return Redirect("/Panel/CompanyDataManage/Products/?bid=" + request.BusinessId);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct([FromQuery] string lang, int Id)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var item = iITCFServ.iProductServ.Find(x => x.Id == Id);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == item.BusinessId && x.CreatorId == theUser.Id);
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
            request.Description = item.Description;
            request.Image = item.Image;
            request.Video = item.Video;
            request.GroupId = item.GroupId;
            request.BusinessId = item.BusinessId;
            DropDownBinder(request);
            return View(GetViewName(lang, "EditProduct"), request);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct([FromQuery] string lang, ProductRequest request)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            var item = iITCFServ.iProductServ.Find(x => x.Id == request.Id);
            var theBusiness = iITCFServ.IBusinessServ.Find(x => x.Id == request.BusinessId && x.CreatorId == theUser.Id);
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

            item.Title = request.Title;
            item.Description = request.Description;
            if (!string.IsNullOrEmpty(Image))
                item.Image = Image;
            if (!string.IsNullOrEmpty(Video))
                item.Video = Video;
            await iITCFServ.iProductGroupServ.SaveChangesAsync();
            return Redirect("/Panel/CompanyDataManage/Products/?bid=" + request.BusinessId);
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
            return Redirect("/Panel/CompanyDataManage/Products/?bid=" + bid);
        }

        private void DropDownBinder(ProductRequest request)
        {
            var countries = iITCFServ.iProductGroupServ.GetAll(x => x.BusinessId == request.BusinessId);
            ViewBag.Groups = new SelectList(countries, "Id", "Title", request?.BusinessId);
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

            if (request.GroupId == 0)
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
    }
}
