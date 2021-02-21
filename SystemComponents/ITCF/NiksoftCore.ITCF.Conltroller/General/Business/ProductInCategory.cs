using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.ITCF.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using System.Linq;

namespace NiksoftCore.ITCF.Conltroller.General.Business
{
    public class ProductInCategory : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        public IITCFService iITCFServ { get; set; }

        public ProductInCategory(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            iITCFServ = new ITCFService(Configuration);
        }

        public IActionResult Index([FromQuery] ProductSearchRequest request)
        {
            if (!string.IsNullOrEmpty(request.lang))
                request.lang = request.lang.ToLower();
            else
                request.lang = defaultLang.ShortName.ToLower();

            if (request.lang == "fa")
                ViewBag.PageTitle = "محصولات";
            else
                ViewBag.PageTitle = "Products";

            var query = iITCFServ.iProductServ.ExpressionMaker();
            query.Add(x => true);

            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title) || x.Description.Contains(request.Title));
            }

            if (request.CategoryId > 0)
            {
                query.Add(x => x.CategoryId == request.CategoryId);
            }

            if (request.BusinessId > 0)
            {
                query.Add(x => x.BusinessId == request.BusinessId);
            }

            var total = iITCFServ.iProductServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iITCFServ.iProductServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            BindCombos(request);
            return View(GetViewName(request.lang, "Index"));
        }

        public IActionResult SingleProduct(int Id, string lang)
        {
            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            var theProduct = iITCFServ.iProductServ.Find(x => x.Id == Id);
            ViewBag.Product = theProduct;

            if (lang == "fa")
                ViewBag.PageTitle = theProduct.Title;
            else
                ViewBag.PageTitle = theProduct.Title;

            return View(GetViewName(lang, "SingleProduct"));
        }

        private void BindCombos(ProductSearchRequest request)
        {
            var categories = iITCFServ.IBusinessCategoryServ.GetAll(x => x.ParentId == null).OrderBy(x => x.Title).ToList();
            categories.Insert(0, new BusinessCategory
            {
                Id = 0,
                Title = "دسته بندی ها"
            });
            ViewBag.Categories = new SelectList(categories, "Id", "Title", request?.CategoryId);
        }


    }
}