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

namespace NiksoftCore.SystemBase.Controllers.Panel.Widgets
{
    public class WUserInfo : ViewComponent
    {
        public IConfiguration Config { get; }
        public ISystemBaseService ISystemBaseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;

        public List<NikMessage> Messages;

        public WUserInfo(IConfiguration Configuration, UserManager<DataModel.User> userManager)
        {
            Config = Configuration;
            Messages = new List<NikMessage>();
            ISystemBaseServ = new SystemBaseService(Config.GetConnectionString("SystemBase"));
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var thisUser = await userManager.GetUserAsync(HttpContext.User);
                var thisProfile = await ISystemBaseServ.iUserProfileServ.FindAsync(x => x.UserId == thisUser.Id);

                ViewBag.Profile = thisProfile;
                ViewBag.User = thisUser;
            }

            return View();
        }

    }
}
