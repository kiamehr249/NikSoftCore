using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.ITCF.Service;
using NiksoftCore.MiddlController.Middles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.ITCF.Conltroller.General.Business
{
    [Area("Business")]
    public class PaymentStep : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        public IITCFService iITCFServ { get; set; }

        public PaymentStep(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            iITCFServ = new ITCFService(Configuration);
        }

        [HttpGet]
        public async Task<IActionResult> Index(int Id)
        {
            var thisUser = await userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = thisUser;
            ViewBag.PageTitle = "پیش پرداخت سفارش";
            ViewBag.Content = iITCFServ.iUserPurchaseServ.Find(x => x.Id == Id);
            ViewBag.Profile = iITCFServ.iUserProfileServ.Find(x => x.UserId == thisUser.Id);
            return View();
        }


    }
}