using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.Bourse.Service;
using NiksoftCore.DataModel;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.SystemBase.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.Bourse.Controllers.API
{
    [Route("/api/[controller]/[action]")]
    public class BourseApi : NikApi
    {
        public IConfiguration Configuration { get; }
        public ISystemBaseService iSystemBaseServ { get; set; }
        public IBourseService iBourseServ { get; set; }
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        public User theUser { get; set; }

        public BourseApi(IConfiguration configuration, 
            UserManager<DataModel.User> userManager, 
            SignInManager<User> signInManager)
        {
            Configuration = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
            iSystemBaseServ = new SystemBaseService(Configuration.GetConnectionString("SystemBase"));
            iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
        }

    }
}
