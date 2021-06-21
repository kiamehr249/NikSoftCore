using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.ViewModel;
using System.Diagnostics;
using System.Text;

namespace NiksoftCore.SystemBase.Controllers.General.Home
{
    public class Home : NikController
    {
        private readonly ILogger<Home> _logger;

        public Home(ILogger<Home> logger, IConfiguration Configuration) : base(Configuration)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return Redirect("/Auth/Account/Login");
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult CustomPage()
        {
            return Content(
                @"<!doctype html>
                <html lang='en'>
                    <head>
                        <title>تست نمایش پویا</title>
                        <link rel='stylesheet' href='/templates/UiTemplate/NikFace/rtl/css/bootstrap.rtl.full.min.css' />
                        <script src='/templates/UiTemplate/NikFace/rtl/js/jquery-2.1.4.min.js'></script>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='row'>
                                <div class='col-sm-4'>
                                    <h3>صفحه پویا</h3>
                                </div>
                            </div>
                        </div>
                    </body>
                </html>", "text/html", Encoding.UTF8
            );
        }

    }
}
