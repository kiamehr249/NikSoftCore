using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.ITCF.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.ITCF.Conltroller.General.Business
{
    [Area("Business")]
    public class Company : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        public IITCFService iITCFServ { get; set; }

        public Company(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            iITCFServ = new ITCFService(Configuration);
        }

        public IActionResult Index([FromQuery] BusinessSearchRequest request)
        {
            if (!string.IsNullOrEmpty(request.lang))
                request.lang = request.lang.ToLower();
            else
                request.lang = defaultLang.ShortName.ToLower();

            var total = iITCFServ.IBusinessServ.Count(x => x.Status == BusinessStatus.ConfirmShow);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;



            if (request.lang == "fa")
                ViewBag.PageTitle = "کسب و کارها";
            else
                ViewBag.PageTitle = "Companies";

            var query = iITCFServ.IBusinessServ.ExpressionMaker();
            query.Add(x => x.Status == BusinessStatus.ConfirmShow);

            if (!string.IsNullOrEmpty(request.CoName))
            {
                query.Add(x => x.CoName.Contains(request.CoName));
            }

            if (request.ProvinceId > 0)
            {
                query.Add(x => x.ProvinceId == request.ProvinceId);
            }

            if (request.CityId > 0)
            {
                query.Add(x => x.CityId == request.CityId);
            }

            ViewBag.Contents = iITCFServ.IBusinessServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            BindCombos(request);
            return View(GetViewName(request.lang, "Index"));
        }

        public IActionResult SingleCompany(BusinessSearchRequest request)
        {
            if (!string.IsNullOrEmpty(request.lang))
                request.lang = request.lang.ToLower();
            else
                request.lang = defaultLang.ShortName.ToLower();

            var TheCompany = iITCFServ.IBusinessServ.Find(x => x.Id == request.Id);
            ViewBag.TheCom = TheCompany;

            if (request.lang == "fa")
                ViewBag.PageTitle = TheCompany.CoName;
            else
                ViewBag.PageTitle = TheCompany.CoName;

            ViewBag.Contents = iITCFServ.iIntroductionServ.GetAll(x => x.BusinessId == TheCompany.Id).ToList();
            ViewBag.Products = iITCFServ.iProductServ.GetPart(x => x.BusinessId == TheCompany.Id, 0, 8).ToList();
            return View(GetViewName(request.lang, "SingleCompany"));
        }


        public IActionResult Categories(CategorySearchRequest request)
        {
            if (!string.IsNullOrEmpty(request.lang))
                request.lang = request.lang.ToLower();
            else
                request.lang = defaultLang.ShortName.ToLower();

            var total = iITCFServ.IBusinessCategoryServ.Count(x => true);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;



            if (request.lang == "fa")
                ViewBag.PageTitle = "دسته بندی ها";
            else
                ViewBag.PageTitle = "Categories";

            var query = iITCFServ.IBusinessCategoryServ.ExpressionMaker();
            query.Add(x => x.ParentId == null);

            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
            }

            ViewBag.Contents = iITCFServ.IBusinessCategoryServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View(GetViewName(request.lang, "Categories"));
        }

        public async Task<IActionResult> CategoryCompanies([FromQuery] BusinessSearchRequest request)
        {
            if (!string.IsNullOrEmpty(request.lang))
                request.lang = request.lang.ToLower();
            else
                request.lang = defaultLang.ShortName.ToLower();

            var theCategory = await iITCFServ.IBusinessCategoryServ.FindAsync(x => x.Id== request.CatgoryId);
            ViewBag.Category = theCategory;

            var total = iITCFServ.IBusinessServ.Count(x => x.Status == BusinessStatus.ConfirmShow && x.CatgoryId == request.CatgoryId);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;



            if (request.lang == "fa")
                ViewBag.PageTitle = "کسب و کارها";
            else
                ViewBag.PageTitle = "Companies";

            var query = iITCFServ.IBusinessServ.ExpressionMaker();
            query.Add(x => x.Status == BusinessStatus.ConfirmShow && x.CatgoryId == request.CatgoryId);

            if (!string.IsNullOrEmpty(request.CoName))
            {
                query.Add(x => x.CoName.Contains(request.CoName));
            }

            if (request.ProvinceId > 0)
            {
                query.Add(x => x.ProvinceId == request.ProvinceId);
            }

            if (request.CityId > 0)
            {
                query.Add(x => x.CityId == request.CityId);
            }

            ViewBag.Contents = iITCFServ.IBusinessServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            BindCombos(request);
            return View(GetViewName(request.lang, "CategoryCompanies"));
        }

        private void BindCombos(BusinessSearchRequest request)
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