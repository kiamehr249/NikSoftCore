using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.ITCF.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.ITCF.Conltroller.Widgets
{
    public class WIndustrialPark : ViewComponent
    {
        public IITCFService IITCFServ { get; set; }

        public WIndustrialPark(IConfiguration Configuration, UserManager<DataModel.User> userManager)
        {
            IITCFServ = new ITCFService(Configuration);
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.Parks = IITCFServ.IIndustrialParkServ.GetPart(x =>
            true, 0, 10);
            return View();
        }

    }
}
