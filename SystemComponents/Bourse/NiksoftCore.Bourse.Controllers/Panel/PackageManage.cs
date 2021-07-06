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
    public class PackageManage : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly IWebHostEnvironment hosting;
        public IBourseService iBourseServ { get; set; }

        public PackageManage(IConfiguration Configuration, IWebHostEnvironment hostingEnvironment,
            UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            hosting = hostingEnvironment;
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
        }

        public IActionResult Index(PackageSearch request)
        {
            ViewBag.PageTitle = "Pricing Packages Management";

            var query = iBourseServ.iPricingPackageServ.ExpressionMaker();
            query.Add(x => true);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            ViewBag.Search = isSearch;


            var total = iBourseServ.iPricingPackageServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iBourseServ.iPricingPackageServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View(request);
        }

        [HttpGet]
        public IActionResult Create(int Id)
        {

            ViewBag.PageTitle = "Creat Pricing Package";

            var request = new PackageRequest();

            if (Id > 0)
            {
                var item = iBourseServ.iPricingPackageServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.Pric = item.Pric;
                request.ColorHex = item.ColorHex;
                request.ListObjects = item.ListObjects;
                request.Description = item.Description;
            }
            else
            {
                request.ListObjects = "[]";
            }


            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PackageRequest request)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (!PackageValide(request))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            PricingPackage item;

            if (request.Id > 0)
            {
                item = iBourseServ.iPricingPackageServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }
            else
            {
                item = new PricingPackage();
            }

            item.Title = request.Title;
            item.Pric = request.Pric;
            item.Description = request.Description;
            item.ColorHex = request.ColorHex;
            item.ListObjects = request.ListObjects;
            request.Description = request.Description;

            if (request.Id == 0)
            {
                item.CreatedBy = user.Id;
                item.CreateDate = DateTime.Now;
                iBourseServ.iPricingPackageServ.Add(item);
            }


            await iBourseServ.iPricingPackageServ.SaveChangesAsync();
            return Redirect("/Panel/PackageManage");

        }

        public async Task<IActionResult> Remove(int Id)
        {
            var theContent = iBourseServ.iPricingPackageServ.Find(x => x.Id == Id);
            iBourseServ.iPricingPackageServ.Remove(theContent);
            await iBourseServ.iPricingPackageServ.SaveChangesAsync();
            return Redirect("/Panel/PackageManage");
        }

        private bool PackageValide(PackageRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("The Title must have a value", "fa");
            }

            if (request.Pric == 0)
            {
                AddError("The Pric must have a value", "fa");
            }

            if (string.IsNullOrEmpty(request.ColorHex))
            {
                AddError("The Color Hex Code must have a value", "fa");
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
