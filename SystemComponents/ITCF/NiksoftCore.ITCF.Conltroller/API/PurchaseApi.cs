using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.ITCF.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.ITCF.Conltroller.API
{
    [Microsoft.AspNetCore.Mvc.Route("/api/[controller]/[action]")]
    public class PurchaseApi : NikApi
    {
        public IConfiguration Configuration { get; }
        public IITCFService iITCFServ { get; set; }

        private readonly UserManager<DataModel.User> UserManager;

        public PurchaseApi(IConfiguration configuration, UserManager<DataModel.User> userManager)
        {
            Configuration = configuration;
            UserManager = userManager;
            iITCFServ = new ITCFService(Configuration);
        }

        [HttpPost]
        public async Task<IActionResult> SetBasketItem([FromBody] SetBasketRequest request)
        {
            var theUser = await UserManager.GetUserAsync(HttpContext.User);
            if (theUser == null)
            {
                return Ok(new
                {
                    status = 401,
                    message = "عدم دسترسی به سرویس"
                });
            }

            if (request.productId == 0)
            {
                return Ok(new
                {
                    status = 500,
                    message = "خطا در مقادیر ورودی"
                });
            }

            if (request.countOf == 0)
            {
                return Ok(new
                {
                    status = 500,
                    message = "خطا در مقادیر ورودی"
                });
            }

            var theProduct = await iITCFServ.iProductServ.FindAsync(x => x.Id == request.productId);

            if (theProduct == null)
            {
                return Ok(new
                {
                    status = 500,
                    message = "خطا در مقادیر ورودی"
                });
            }

            var newItem = new UserPurchase
            {
                UserId = theUser.Id,
                ProductId = theProduct.Id,
                Count = request.countOf,
                DeliveryType = DeliveryType.Default,
                Status = PurchaseStatus.Requested,
                CreateDate = DateTime.Now
            };

            iITCFServ.iUserPurchaseServ.Add(newItem);

            await iITCFServ.iUserPurchaseServ.SaveChangesAsync();

            return Ok(new
            {
                status = 200,
                message = "ثبت موفق",
                data = new
                {
                    Id = newItem.Id,
                    UserId = theUser.Id,
                    ProductId = theProduct.Id,
                    Count = request.countOf,
                    Image = theProduct.Image,
                    Title = theProduct.Title
                }
            });
        }


        [HttpPost]
        public async Task<IActionResult> GetBasketItems()
        {
            var theUser = await UserManager.GetUserAsync(HttpContext.User);
            if (theUser == null)
            {
                return Ok(new
                {
                    status = 401,
                    message = "عدم دسترسی به سرویس"
                });
            }

            var basketItems = iITCFServ.iUserPurchaseServ.GetAll(x => x.UserId == theUser.Id && x.Status == PurchaseStatus.Requested, y => new { 
                y.Id,
                y.UserId,
                y.ProductId,
                y.Count,
                Image = y.Product.Image,
                Title = y.Product.Title
            }).ToList();

            return Ok(new
            {
                status = 200,
                message = "دریافت موفق",
                data = basketItems
            });
        }


        [HttpPost]
        public async Task<IActionResult> RemoveBasket()
        {
            var theUser = await UserManager.GetUserAsync(HttpContext.User);
            if (theUser == null)
            {
                return Ok(new
                {
                    status = 401,
                    message = "عدم دسترسی به سرویس"
                });
            }

            var basketItems = iITCFServ.iUserPurchaseServ.GetAll(x => x.UserId == theUser.Id && x.Status == PurchaseStatus.Requested).ToList();

            foreach (var item in basketItems)
            {
                if (item.Status == PurchaseStatus.Requested)
                {
                    iITCFServ.iUserPurchaseServ.Remove(item);
                }
            }

            await iITCFServ.iUserPurchaseServ.SaveChangesAsync();

            return Ok(new
            {
                status = 200,
                message = "دریافت موفق",
                data = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBasketItem([FromBody] RemoveRequest request)
        {
            var theUser = await UserManager.GetUserAsync(HttpContext.User);
            if (theUser == null)
            {
                return Ok(new
                {
                    status = 401,
                    message = "عدم دسترسی به سرویس"
                });
            }

            var basketItem = iITCFServ.iUserPurchaseServ.Find(x => x.Id == request.id);
            if (basketItem == null)
            {
                return Ok(new
                {
                    status = 404,
                    message = "چنین سفارشی وجود ندارد"
                });
            }

            if (basketItem.Status != PurchaseStatus.Requested)
            {
                return Ok(new
                {
                    status = 500,
                    message = "امکان حذف این سفارش وجود ندارد."
                });
            }

            iITCFServ.iUserPurchaseServ.Remove(basketItem);
            await iITCFServ.iUserPurchaseServ.SaveChangesAsync();

            return Ok(new
            {
                status = 200,
                message = "دریافت موفق",
                data = request.id
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetBasketAll()
        {
            var theUser = await UserManager.GetUserAsync(HttpContext.User);
            if (theUser == null)
            {
                return Ok(new
                {
                    status = 401,
                    message = "عدم دسترسی به سرویس"
                });
            }

            var basketItems = iITCFServ.iUserPurchaseServ.GetAll(x => x.UserId == theUser.Id && (x.Status == PurchaseStatus.Requested || x.Status == PurchaseStatus.Confirmed), y => new {
                y.Id,
                y.UserId,
                y.ProductId,
                y.PrePayment,
                y.UnitAmount,
                y.TransportAmount,
                y.Count,
                y.Status,
                y.DeliveryType,
                Image = y.Product.Image,
                Title = y.Product.Title
            }).ToList();

            return Ok(new
            {
                status = 200,
                message = "دریافت موفق",
                data = basketItems
            });
        }

    }
}
