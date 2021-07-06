using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.DataModel;
using NiksoftCore.FormBuilder.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.SystemBase.Service;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.FormBuilder.Controller.APIs
{
    [Route("/api/[controller]/[action]")]
    public class FormBuilderApi : NikApi
    {
        public IConfiguration Config { get; }
        public ISystemBaseService iSystemBaseServ { get; set; }
        private IFormBuilderService iFormBuilderServ { get; set; }
        private readonly IWebHostEnvironment hosting;

        public User theUser { get; set; }

        private readonly UserManager<User> userManager;

        public FormBuilderApi(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, 
            UserManager<DataModel.User> userManager, SignInManager<User> signInManager)
        {
            Config = configuration;
            this.userManager = userManager;
            iSystemBaseServ = new SystemBaseService(Config.GetConnectionString("SystemBase"));
            hosting = hostingEnvironment;
            iFormBuilderServ = new FormBuilderService(Config.GetConnectionString("SystemBase"));
        }


        [HttpPost]
        public async Task<IActionResult> FileUpload()
        {
            var file = Request.Form.Files[0];

            if (file == null || file.Length == 0)
            {
                return Ok(new
                {
                    status = 404,
                    message = "No file selected",
                    data = ""
                });
            }

            string filePath = string.Empty;
            if (file != null && file.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = file,
                    RootPath = hosting.ContentRootPath,
                    UnitPath = Config.GetSection("FileRoot:FormDir").Value
                });

                if (!SaveImage.Success)
                {
                    return Ok(new
                    {
                        status = 404,
                        message = "File upload failed Try again",
                        data = ""
                    });
                }

                filePath = SaveImage.FilePath;
            }


            return Ok(new
            {
                status = 200,
                message = "File uploaded",
                data = filePath
            });
        }





    }
}
