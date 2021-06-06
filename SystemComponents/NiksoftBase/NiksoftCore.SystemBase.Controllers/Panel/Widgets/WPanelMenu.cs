using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.SystemBase.Service;
using NiksoftCore.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.SystemBase.Controllers.Panel.Widgets
{
    public class WPanelMenu : ViewComponent
    {
        public IConfiguration Config { get; }
        public ISystemBaseService ISystemBaseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;

        public List<NikMessage> Messages;

        public WPanelMenu(IConfiguration Configuration, UserManager<DataModel.User> userManager)
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
                var userRoles = await userManager.GetRolesAsync(thisUser);

                ViewBag.Roles = userRoles.ToList();

                var menus = ISystemBaseServ.iPanelMenuService.GetPart(x => x.Enabled && x.ParentId == null, 0, 50).OrderBy(x => x.Ordering).ToList();
                List<PanelMenu> permits = new List<PanelMenu>();
                foreach (var menu in menus)
                {
                    if (string.IsNullOrEmpty(menu.Roles))
                    {
                        permits.Add(menu);
                    }
                    else
                    {
                        if (menu.Roles.Contains(","))
                        {
                            var readies = menu.Roles.Split(',');
                            //bool oneAdd = false;
                            foreach (var item in readies)
                            {
                                if (userRoles.Contains(item))
                                {
                                    permits.Add(menu);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (userRoles.Contains(menu.Roles))
                            {
                                permits.Add(menu);
                            }
                        }

                    }
                }

                ViewBag.Menus = permits;
            }


            return View();
        }

    }
}
