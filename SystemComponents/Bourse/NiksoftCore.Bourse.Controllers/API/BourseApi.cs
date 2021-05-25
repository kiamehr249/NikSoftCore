using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.Bourse.Service;
using NiksoftCore.DataModel;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.SystemBase.Service;
using System.Collections.Generic;
using System.Linq;

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

        [HttpPost]
        public IActionResult GetFees(int type)
        {

            if (type == 0)
            {
                return Ok(new
                {
                    status = 404,
                    message = "خطا در مقادیر ورودی",
                    data = new List<string>()
                });
            }

            var feeType = (FeeType)type;

            var fees = iBourseServ.iFeeServ.GetAll(x => x.FeeType == feeType, y => new {
                y.Id,
                Title = y.Title + " از " + y.FromAmount + " تا " + y.ToAmount
            }).ToList();

            return Ok(new
            {
                status = 200,
                message = "دریافت اطلاعات",
                data = fees
            });
        }

    }
}
