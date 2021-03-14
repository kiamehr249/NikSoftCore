using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.ITCF.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace NiksoftCore.ITCF.Conltroller.General.Business
{
    [Area("Business")]
    public class ProductInCategory : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        public IITCFService iITCFServ { get; set; }

        public ProductInCategory(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            iITCFServ = new ITCFService(Configuration);
        }

        public IActionResult Index(ProductSearchRequest request)
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


            ViewBag.Parent = null;
            if (request.CategoryId > 0)
            {
                var firstCat = iITCFServ.IBusinessCategoryServ.Find(x => x.Id == request.CategoryId);
                ViewBag.Parent = firstCat;
                List<int> catIds = new List<int>();
                catIds.Add(request.CategoryId);
                foreach (var subcat in firstCat.Childs)
                {
                    catIds.Add(subcat.Id);
                    foreach (var subcat2 in subcat.Childs)
                    {
                        catIds.Add(subcat2.Id);
                    }

                }

                query.Add(x => catIds.Contains(x.CategoryId));

            }

            if (request.BusinessId > 0)
            {
                query.Add(x => x.BusinessId == request.BusinessId);
            }

            var total = iITCFServ.iProductServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;

            if (request.OrderType == 1)
            {
                ViewBag.Contents = iITCFServ.iProductServ.GetPart(query, pager.StartIndex, pager.PageSize, x => x.Id, true).ToList();
            }
            else if (request.OrderType == 2)
            {
                
            }
            else
            {
                ViewBag.Contents = iITCFServ.iProductServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            }





            var query2 = iITCFServ.IBusinessCategoryServ.ExpressionMaker();
            query2.Add(x => x.Enabled);
            if (request.CategoryId != 0)
            {
                query2.Add(x => x.ParentId == request.CategoryId);
            }
            else
            {
                query2.Add(x => x.ParentId == null);
            }

            ViewBag.Categories = iITCFServ.IBusinessCategoryServ.GetAll(query2).ToList();
            return View(request);
        }

        public IActionResult SingleProduct(int Id)
        {

            var theProduct = iITCFServ.iProductServ.Find(x => x.Id == Id);
            ViewBag.Product = theProduct;

            var SameProducts = iITCFServ.iProductServ.GetAll(x => x.BusinessId == theProduct.BusinessId).ToList();
            ViewBag.SameProducts = SameProducts;

            ViewBag.PageTitle = theProduct.Title;

            return View();
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