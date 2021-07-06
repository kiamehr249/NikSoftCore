using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.Bourse.Service;
using NiksoftCore.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NiksoftCore.Bourse.Controllers.Widget
{
    public class WIcoCases : ViewComponent
    {
        public IConfiguration Config { get; }
        public IBourseService iBourseServ { get; set; }

        public List<NikMessage> Messages;

        public WIcoCases(IConfiguration Configuration)
        {
            Config = Configuration;
            Messages = new List<NikMessage>();
            iBourseServ = new BourseService(Config.GetConnectionString("SystemBase"));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var icoCases = iBourseServ.iIcoCaseServ.GetPart(x => true, 0, 12, x => x.Id, true);
            ViewBag.Contents = icoCases;
            return View();
        }

    }
}
