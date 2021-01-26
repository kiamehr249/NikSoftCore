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
    public class WCopmanyLogos : ViewComponent
    {
        public IITCFService IITCFServ { get; set; }

        public WCopmanyLogos(IConfiguration Configuration)
        {
            IITCFServ = new ITCFService(Configuration);
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.Logos = IITCFServ.IBusinessServ.GetPart(x => true, 0, 6);
            return View();
        }

    }
}
