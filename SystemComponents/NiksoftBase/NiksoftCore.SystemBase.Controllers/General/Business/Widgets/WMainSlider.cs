using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.SystemBase.Service;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.SystemBase.Controllers.General.Business.Widgets
{
    public class WMainSlider : ViewComponent
    {
        public IConfiguration Config { get; }
        public ISystemBaseService iSystemBaseServ { get; set; }

        public List<NikMessage> Messages;
        public PortalLanguage defaultLang;

        public WMainSlider(IConfiguration Configuration)
        {
            Config = Configuration;
            Messages = new List<NikMessage>();
            iSystemBaseServ = new SystemBaseService(Config.GetConnectionString("SystemBase"));
            defaultLang = iSystemBaseServ.iPortalLanguageServ.Find(x => x.IsDefault);
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var Contents = iSystemBaseServ.iGeneralContentServ.GetAll(x => x.ContentCategory.KeyValue == "b_bigslider");
            ViewBag.BigSlider = Contents;
            return View();
        }

    }
}
