using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.Bourse.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.Bourse.Controllers.Panel
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin,Admin")]
    public class IcoCaseManage : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly IWebHostEnvironment hosting;
        public IBourseService iBourseServ { get; set; }

        public IcoCaseManage(IConfiguration Configuration, IWebHostEnvironment hostingEnvironment,
            UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            hosting = hostingEnvironment;
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
        }

        public IActionResult Index(IcoCaseSearch request)
        {
            ViewBag.PageTitle = "ICOs Case Management";

            var query = iBourseServ.iIcoCaseServ.ExpressionMaker();
            query.Add(x => true);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            ViewBag.Search = isSearch;


            var total = iBourseServ.iIcoCaseServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iBourseServ.iIcoCaseServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View(request);
        }

        [HttpGet]
        public IActionResult Create(int Id)
        {

            ViewBag.PageTitle = "Creat ICOs Case";

            var request = new IcoCaseRequest();

            if (Id > 0)
            {
                var item = iBourseServ.iIcoCaseServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.Collected = item.Collected;
                request.StartDate = item.StartDate.ToString("dd/MM/yyyy");
                request.Participants = item.Participants;
                request.FullText = item.FullText;
                request.Icon = item.Icon;
                request.Image = item.Image;
                request.ListObjects = item.ListObjects;
            }
            else
            {
                request.ListObjects = "[]";
            }


            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IcoCaseRequest request)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (!IcoCaseValide(request))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }


            string Icon = string.Empty;
            if (request.IconFile != null && request.IconFile.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.IconFile,
                    RootPath = hosting.ContentRootPath,
                    UnitPath = Config.GetSection("FileRoot:ContentDir").Value
                });

                if (!SaveImage.Success)
                {
                    AddError("File upload failed Try again");
                    ViewBag.Messages = Messages;
                    return View(request);
                }

                Icon = SaveImage.FilePath;
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
                    AddError("File upload failed Try again");
                    ViewBag.Messages = Messages;
                    return View(request);
                }

                Image = SaveImage.FilePath;
            }



            IcoCase item;

            if (request.Id > 0)
            {
                item = iBourseServ.iIcoCaseServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }
            else
            {
                item = new IcoCase();
            }

            item.Title = request.Title;
            item.Collected = request.Collected;
            item.StartDate = Convert.ToDateTime(request.StartDate);
            item.Participants = request.Participants;
            item.FullText = request.FullText;

            if (!string.IsNullOrEmpty(Icon))
                item.Icon = Icon;

            if (!string.IsNullOrEmpty(Image))
                item.Image = Image;

            item.ListObjects = request.ListObjects;

            if (request.Id == 0)
            {
                item.CreatedBy = user.Id;
                item.CreateDate = DateTime.Now;
                iBourseServ.iIcoCaseServ.Add(item);
            }


            await iBourseServ.iIcoCaseServ.SaveChangesAsync();
            return Redirect("/Panel/IcoCaseManage");

        }

        public async Task<IActionResult> Remove(int Id)
        {
            var theContent = iBourseServ.iIcoCaseServ.Find(x => x.Id == Id);
            iBourseServ.iIcoCaseServ.Remove(theContent);
            await iBourseServ.iIcoCaseServ.SaveChangesAsync();
            return Redirect("/Panel/IcoCaseManage");
        }

        private bool IcoCaseValide(IcoCaseRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("The Title must have a value", "fa");
            }

            if (string.IsNullOrEmpty(request.Collected))
            {
                AddError("The Collected must have a value", "fa");
            }

            if (string.IsNullOrEmpty(request.StartDate))
            {
                AddError("The StartDate must have a value", "fa");
            }

            if (string.IsNullOrEmpty(request.Participants))
            {
                AddError("The Participants must have a value", "fa");
            }

            if (Messages.Count(x => x.Type == MessageType.Error) > 0)
            {
                return false;
            }

            return true;
        }

        private void DropDownBinder(ContentRequest request)
        {
            var packageType = new List<ListItemModel>();
            packageType.Add(new ListItemModel
            {
                Id = 0,
                Title = "select type"
            });
            packageType.Add(new ListItemModel
            {
                Id = 1,
                Title = "Basic Backage"
            });
            packageType.Add(new ListItemModel
            {
                Id = 2,
                Title = "Legal Backage"
            });
            packageType.Add(new ListItemModel
            {
                Id = 3,
                Title = "Technology Backage"
            });
            var categories = ISystemBaseServ.iContentCategoryServ.GetAll(x => true);
            ViewBag.Categories = new SelectList(categories, "Id", "Title", request?.CategoryId);
        }



    }
}
