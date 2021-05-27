using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System.Linq;

namespace NiksoftCore.SystemBase.Controllers.General.News
{
    public class News : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly IWebHostEnvironment hosting;
        public News(IConfiguration Configuration, IWebHostEnvironment hostingEnvironment,
            UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            hosting = hostingEnvironment;
        }

        public IActionResult Index(ContentGridRequest request)
        {
            var query = ISystemBaseServ.iGeneralContentServ.ExpressionMaker();
            query.Add(x => x.ContentCategory.KeyValue == "m_part5");

            var total = ISystemBaseServ.iGeneralContentServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = ISystemBaseServ.iGeneralContentServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();

            ViewData["Title"] = "رویدادها";

            return View();
        }

        public IActionResult Single(int Id)
        {
            var theNews = ISystemBaseServ.iGeneralContentServ.Find(x => x.Id == Id);
            ViewBag.Content = theNews;

            ViewData["Title"] = theNews.Title;
            ViewBag.PageTitle = theNews.Title;

            return View();
        }


    }
}
