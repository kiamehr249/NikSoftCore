using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.ITCF.Service;
using System.Threading.Tasks;

namespace NiksoftCore.ITCF.Conltroller.Widgets
{
    public class WBasket : ViewComponent
    {
        private readonly UserManager<DataModel.User> userManager;
        public IITCFService IITCFServ { get; set; }

        public WBasket(IConfiguration Configuration, UserManager<DataModel.User> userManager)
        {
            this.userManager = userManager;
            IITCFServ = new ITCFService(Configuration);
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.Logined = HttpContext.User.Identity.IsAuthenticated;
            return View();
        }

    }
}
