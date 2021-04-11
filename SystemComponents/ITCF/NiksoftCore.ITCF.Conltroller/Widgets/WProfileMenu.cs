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
    public class WProfileMenu : ViewComponent
    {
        private readonly UserManager<DataModel.User> userManager;
        public IITCFService IITCFServ { get; set; }

        public WProfileMenu(IConfiguration Configuration, UserManager<DataModel.User> userManager)
        {
            this.userManager = userManager;
            IITCFServ = new ITCFService(Configuration);
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }

    }
}
