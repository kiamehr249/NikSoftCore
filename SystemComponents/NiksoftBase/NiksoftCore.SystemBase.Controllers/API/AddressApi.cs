using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.SystemBase.Service;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.SystemBase.Controllers.API
{
    [Microsoft.AspNetCore.Mvc.Route("/api/base/[controller]/[action]")]
    public class AddressApi : NikApi
    {
        public IConfiguration Configuration { get; }
        public ISystemBaseService iSystemBaseServ { get; set; }

        private readonly UserManager<DataModel.User> UserManager;

        public AddressApi(IConfiguration configuration, UserManager<DataModel.User> userManager)
        {
            Configuration = configuration;
            UserManager = userManager;
            iSystemBaseServ = new SystemBaseService(Configuration.GetConnectionString("SystemBase"));
        }

        [HttpPost]
        public IActionResult GetCity([FromForm] int provinceId)
        {
            var query = iSystemBaseServ.iCityServ.ExpressionMaker();

            if (provinceId != 0)
            {
                query.Add(x => x.ProvinceId == provinceId);
            }

            var cities = iSystemBaseServ.iCityServ.GetAll(query, y => new {
                y.Id,
                y.Title,
                y.CountryId,
                y.ProvinceId
            }).ToList();

            return Ok(new
            {
                status = 200,
                message = "دریافت موفق",
                count = cities.Count,
                data = cities
            });
        }

        [HttpPost]
        //[Authorize(Policy = "AccessToken")]
        public IActionResult GetProvince([FromForm] int countryId)
        {
            if (countryId == 0)
            {
                return Ok(new
                {
                    status = 500,
                    message = "خطا در مقادیر ورودی"
                });
            }

            var provinces = iSystemBaseServ.iProvinceServ.GetAll(x => x.CountryId == countryId, y => new {
                y.Id,
                y.Title,
                y.CountryId
            }).ToList();

            return Ok(new
            {
                status = 200,
                message = "دریافت موفق",
                count = provinces.Count,
                data = provinces
            });
        }


    }
}