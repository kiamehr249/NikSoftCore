using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.ITCF.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using System;
using System.Linq;
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

            var query = iITCFServ.iUserPurchaseServ.ExpressionMaker();
            query.Add(x => x.Status == request.Status && x.DeliveryType == request.DeliveryType);

            if (!string.IsNullOrEmpty(request.UserName))
            {
                query.Add(x => x.User.UserName.Contains(request.UserName));
            }

            var total = iITCFServ.iUserPurchaseServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;

            ViewBag.PageTitle = "مدیریت درخواست های خرید";

            ViewBag.Contents = iITCFServ.iUserPurchaseServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            ComboBinder(request.DeliveryType, request.Status);
            return View(request);
        }

        public IActionResult CheckRequest(int Id)
        {
            var theRequest = iITCFServ.iUserPurchaseServ.Find(x => x.Id == Id);
            ViewBag.Content = theRequest;
            ViewBag.User = iITCFServ.iUserModelServ.Find(x => x.Id == theRequest.UserId);
            ViewBag.Profile = iITCFServ.iUserProfileServ.Find(x => x.UserId == theRequest.UserId);
            ViewBag.Business = iITCFServ.IBusinessServ.Find(x => x.Id == theRequest.Product.BusinessId);
            var request = new PurchaseCheckRequest();
            request.Id = theRequest.Id;
            request.Status = theRequest.Status;
            request.DeliveryType = theRequest.DeliveryType;
            request.PrePayment = theRequest.PrePayment;
            request.UnitAmount = theRequest.UnitAmount;
            request.Count = theRequest.Count;
            request.TransportAmount = theRequest.TransportAmount;
            request.UserId = theRequest.UserId;
            request.ProductId = theRequest.ProductId;

            ComboBinder(request.DeliveryType, request.Status);
            return View(request);
        }

        [HttpPost]
        public IActionResult CheckRequest(PurchaseCheckRequest request)
        {
            var theRequest = iITCFServ.iUserPurchaseServ.Find(x => x.Id == request.Id);

            theRequest.Status = request.Status;
            theRequest.PrePayment = request.PrePayment;
            theRequest.UnitAmount = request.UnitAmount;
            theRequest.TransportAmount = request.TransportAmount;
            theRequest.DeliveryType = request.DeliveryType;
            theRequest.ModifyDate = DateTime.Now;
            iITCFServ.iUserPurchaseServ.SaveChanges();

            return Redirect("/Panel/PurchaseManage");
        }

        private void ComboBinder(DeliveryType dtype, PurchaseStatus stype)
        {
            var typeList = from PurchaseStatus d in Enum.GetValues(typeof(PurchaseStatus))
                           select new { Id = (int)d, Title = d.GetDisplayName() };
            ViewBag.Statuses = new SelectList(typeList.ToList(), "Id", "Title", (int)stype);

            var DeliveryList = from DeliveryType d in Enum.GetValues(typeof(DeliveryType))
                           select new { Id = (int)d, Title = d.GetDisplayName() };
            ViewBag.DeliveryTypes = new SelectList(DeliveryList.ToList(), "Id", "Title", (int)dtype);
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
