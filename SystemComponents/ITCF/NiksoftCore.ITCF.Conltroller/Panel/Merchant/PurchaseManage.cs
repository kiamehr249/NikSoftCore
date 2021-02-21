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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.ITCF.Conltroller.Panel.Merchant
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin,Admin")]
    public class PurchaseManage : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        public IITCFService iITCFServ { get; set; }
        private readonly IWebHostEnvironment hosting;

        public PurchaseManage(IConfiguration Configuration, IWebHostEnvironment hostingEnvironment,
            UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            iITCFServ = new ITCFService(Configuration);
            hosting = hostingEnvironment;
        }

        public async Task<IActionResult> Index([FromQuery] UserPurchaseGridRequest request)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            if (!string.IsNullOrEmpty(request.lang))
                request.lang = request.lang.ToLower();
            else
                request.lang = defaultLang.ShortName.ToLower();

            var query = iITCFServ.iUserPurchaseServ.ExpressionMaker();
            query.Add(x => x.Status == request.Status);

            if (request.UserId != 0)
            {
                query.Add(x => x.UserId == request.UserId);
            }

            if (request.ProductId != 0)
            {
                query.Add(x => x.ProductId == request.ProductId);
            }

            var total = iITCFServ.iUserPurchaseServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;

            if (request.lang == "fa")
                ViewBag.PageTitle = "مدیریت درخواست های خرید";
            else
                ViewBag.PageTitle = "Purchase Management";

            ViewBag.Contents = iITCFServ.iUserPurchaseServ.GetPart(x => x.Status == PurchaseStatus.Requested, pager.StartIndex, pager.PageSize).ToList();

            return View(GetViewName(request.lang, "Index"));
        }

        public IActionResult CheckRequest(int Id, string lang)
        {
            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            var theRequest = iITCFServ.iUserPurchaseServ.Find(x => x.Id == Id);
            ViewBag.Content = theRequest;
            ViewBag.User = iITCFServ.iUserModelServ.Find(x => x.Id == theRequest.UserId);
            ViewBag.Profile = iITCFServ.iUserProfileServ.Find(x => x.UserId == theRequest.UserId);
            return View(GetViewName(lang, "CheckRequest"));
        }

        [HttpPost]
        public IActionResult CheckRequest(PurchaseCheckRequest request)
        {
            if (!string.IsNullOrEmpty(request.lang))
                request.lang = request.lang.ToLower();
            else
                request.lang = defaultLang.ShortName.ToLower();

            var theRequest = iITCFServ.iUserPurchaseServ.Find(x => x.Id == request.Id);

            theRequest.Status = request.Status;
            theRequest.DeliveryType = request.DeliveryType;

            iITCFServ.iUserPurchaseServ.SaveChanges();

            return Redirect("/Panel/PurchaseManage");
        }

        private void ComboBinder(PurchaseCheckRequest request)
        {
            var typeList = from PurchaseStatus d in Enum.GetValues(typeof(PurchaseStatus))
                           select new { Id = (int)d, Title = d.GetDisplayName() };
            ViewBag.Statuses = new SelectList(typeList.ToList(), "Id", "Title", request.Status);

            var DeliveryList = from DeliveryType d in Enum.GetValues(typeof(DeliveryType))
                           select new { Id = (int)d, Title = d.GetDisplayName() };
            ViewBag.DeliveryTypes = new SelectList(DeliveryList.ToList(), "Id", "Title", request.DeliveryType);
        }

        public IActionResult Reject(int Id)
        {
            var theRequest = iITCFServ.iUserPurchaseServ.Find(x => x.Id == Id);
            theRequest.Status = PurchaseStatus.Rejected;
            iITCFServ.iUserPurchaseServ.SaveChanges();
            return Redirect("/Panel/PurchaseManage");
        }

    }
}
