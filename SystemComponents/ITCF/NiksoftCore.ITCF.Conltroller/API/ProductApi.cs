using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.DataModel;
using NiksoftCore.ITCF.Service;
using NiksoftCore.MiddlController.Middles;
using System;
using System.Threading.Tasks;

namespace NiksoftCore.ITCF.Conltroller.API
{
    [Route("/api/[controller]/[action]")]
    public class ProductApi : NikApi
    {
        public IConfiguration Configuration { get; }
        public IITCFService iITCFServ { get; set; }

        private readonly UserManager<User> userManager;

        public ProductApi(IConfiguration configuration, UserManager<DataModel.User> userManager)
        {
            Configuration = configuration;
            this.userManager = userManager;
            iITCFServ = new ITCFService(Configuration);
        }

        [HttpPost]
        public async Task<IActionResult> SetProductOrder([FromForm] int Id, [FromForm] int Count)
        {
            var theUser = await userManager.GetUserAsync(HttpContext.User);
            if (theUser == null)
            {
                return Ok(new
                {
                    status = 401,
                    message = "برای استفاده از این سرویس باید در سامانه وارد شوید"
                });
            }
            if (Id == 0)
            {
                return Ok(new
                {
                    status = 500,
                    message = "خطا در مقادیر ورودی"
                });
            }

            var theProduct = await iITCFServ.iProductServ.FindAsync(x => x.Id == Id);

            if (theProduct == null)
            {
                return Ok(new
                {
                    status = 404,
                    message = "چنین محصولی وجود ندارد"
                });
            }

            iITCFServ.iUserPurchaseServ.Add(new UserPurchase {
                UserId = theUser.Id,
                ProductId = theProduct.Id,
                Count = Count,
                DeliveryType = DeliveryType.Default,
                CreateDate = DateTime.Now
            });

            await iITCFServ.iUserPurchaseServ.SaveChangesAsync();

            return Ok(new
            {
                status = 200,
                message = "دریافت موفق",
                data = theProduct.Id
            });
        }

    }
}
