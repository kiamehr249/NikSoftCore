using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.ITCF.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace NiksoftCore.ITCF.Conltroller.General.Business
{
    [Area("Business")]
    public class ActiveOrders : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        public IITCFService iITCFServ { get; set; }

        public ActiveOrders(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            iITCFServ = new ITCFService(Configuration);
        }

        public IActionResult Index(ProductSearchRequest request)
        {
            ViewBag.PageTitle = "پیگیری سفارش ها";

            var query = iITCFServ.iUserPurchaseServ.ExpressionMaker();
            query.Add(x => x.Status > PurchaseStatus.Confirmed && x.Status != PurchaseStatus.Delivered);

            var total = iITCFServ.iUserPurchaseServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iITCFServ.iUserPurchaseServ.GetPartOptional(query, pager.StartIndex, pager.PageSize);

            return View();
        }


    }
}