using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.SystemBase.Service;
using NiksoftCore.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NiksoftCore.SystemBase.Controllers.General.Widgets
{
    public class WMenus : ViewComponent
    {
        public IConfiguration Config { get; }
        public ISystemBaseService iSystemBaseServ { get; set; }

        public List<NikMessage> Messages;
        public PortalLanguage defaultLang;

        public WMenus(IConfiguration Configuration)
        {
            Config = Configuration;
            Messages = new List<NikMessage>();
            iSystemBaseServ = new SystemBaseService(Config.GetConnectionString("SystemBase"));
            defaultLang = iSystemBaseServ.iPortalLanguageServ.Find(x => x.IsDefault);
        }

        public async Task<IViewComponentResult> InvokeAsync(string key, int size = 8)
        {
            var theCategory = await iSystemBaseServ.iMenuCategoryServ.FindAsync(x => x.KeyValue == key);
            ViewBag.Contents = theCategory;
            return View("FaMenu");
        }

    }
}
