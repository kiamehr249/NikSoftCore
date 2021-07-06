using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.Bourse.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.Bourse.Controllers.General
{
    public class ICOsPortfolio : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly IWebHostEnvironment hosting;
        public IBourseService iBourseServ { get; set; }

        public ICOsPortfolio(IConfiguration Configuration, IWebHostEnvironment hostingEnvironment,
            UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            hosting = hostingEnvironment;
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
        }

        public IActionResult Index(IcoCaseSearch request)
        {
            ViewBag.PageTitle = "ICOs Portfolio";

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


        public async Task<IActionResult> SinglePortfolio(int Id)
        {
            var theItem = await iBourseServ.iIcoCaseServ.FindAsync(x => x.Id == Id);
            ViewBag.PageTitle = theItem.Title;
            ViewBag.Title = theItem.Title;
            ViewBag.Content = theItem;
            return View();
        }

    }
}
