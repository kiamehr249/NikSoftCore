using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.SystemBase.Service;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.SystemBase.Controllers.General.Widgets
{
    public class WProfileSearch : ViewComponent
    {
        public IConfiguration Config { get; }
        public ISystemBaseService iSystemBaseServ { get; set; }

        public List<NikMessage> Messages;
        public PortalLanguage defaultLang;

        public WProfileSearch(IConfiguration Configuration)
        {
            Config = Configuration;
            Messages = new List<NikMessage>();
            iSystemBaseServ = new SystemBaseService(Config.GetConnectionString("SystemBase"));
            defaultLang = iSystemBaseServ.iPortalLanguageServ.Find(x => x.IsDefault);
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }

    }
}
