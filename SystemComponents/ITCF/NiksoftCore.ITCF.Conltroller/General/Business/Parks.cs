using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.ITCF.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.ITCF.Conltroller.General.Business
{
    [Area("Business")]
    public class Parks : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        public IITCFService iITCFServ { get; set; }

        public Parks(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            iITCFServ = new ITCFService(Configuration);
        }

        public IActionResult Index([FromQuery] ParkSearchRequest request)
        {
            if (!string.IsNullOrEmpty(request.lang))
                request.lang = request.lang.ToLower();
            else
                request.lang = defaultLang.ShortName.ToLower();

            var total = iITCFServ.IIndustrialParkServ.Count(x => true);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;



            if (request.lang == "fa")
                ViewBag.PageTitle = "شهرک های صنعتی";
            else
                ViewBag.PageTitle = "Industrial Parks";

            var query = iITCFServ.IIndustrialParkServ.ExpressionMaker();
            query.Add(x => true);
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
            }

            if (request.ProvinceId != null && request.ProvinceId != 0)
            {
                query.Add(x => x.ProvinceId == request.ProvinceId);
            }

            ViewBag.Contents = iITCFServ.IIndustrialParkServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            BindCombos(request);
            return View(GetViewName(request.lang, "Index"));
        }


        private void BindCombos(ParkSearchRequest request)
        {
            var provinces = iITCFServ.iProvinceServ.GetAll(x => x.CountryId == 1).OrderBy(x => x.Title).ToList();
            provinces.Insert(0, new Province
            {
                Id = 0,
                Title = "استان ها"
            });
            ViewBag.Provinces = new SelectList(provinces, "Id", "Title", request?.ProvinceId);
        }


    }
}
