using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.MiddlController.Middles;

namespace NiksoftCore.SystemBase.Controllers.Panel
{
    [Authorize]
    [Area("Panel")]
    public class Home : NikController
    {

        public Home(IConfiguration Configuration) : base(Configuration)
        {
        }

        public IActionResult Index()
        {
            ViewBag.PageTitle = "Dashboard";
            return View();
        }
    }
}
