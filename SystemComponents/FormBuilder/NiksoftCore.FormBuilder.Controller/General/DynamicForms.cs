using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NiksoftCore.FormBuilder.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.FormBuilder.Controller.General
{
    public class DynamicForms : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly IWebHostEnvironment hosting;
        private IFormBuilderService iFormBuilderServ { get; set; }

        public DynamicForms(IConfiguration Configuration, IWebHostEnvironment hostingEnvironment,
            UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            hosting = hostingEnvironment;
            iFormBuilderServ = new FormBuilderService(Configuration.GetConnectionString("SystemBase"));
        }

        public async Task<IActionResult> Index(int FormId)
        {
            var theForm = await iFormBuilderServ.iFormServ.FindAsync(x => x.Id == FormId);
            var controls = iFormBuilderServ.iFormControlServ.GetPart(x => x.FormId == FormId, 0, 30, x => x.OrderId, false);
            ViewBag.Form = theForm;
            List<FormAnswer> Answers = new List<FormAnswer>();
            var request = new FormDataRequest();


            if (theForm.LoginRequired)
            {
                if (!User.Identity.IsAuthenticated)
                {
                    Redirect("/Auth/Account/Login");
                }
            }
            else
            {
                var theUser = await userManager.GetUserAsync(HttpContext.User);
                //var theAnswer = await iFormBuilderServ.iFormDataServ.FindAsync(x => x.UserId == theUser.Id);
                //if (theAnswer != null)
                //{
                //    Answers = JsonConvert.DeserializeObject<List<FormAnswer>>(theAnswer.AnswerObject);
                //}

            }

            foreach (var ctrl in controls)
            {
                var items = new List<AnswerItem>();
                if (ctrl.ControlType == ControlType.DropDown || ctrl.ControlType == ControlType.RadioList)
                {
                    foreach (var citem in ctrl.ControlItems)
                    {
                        items.Add(new AnswerItem { 
                            ItemValue = citem.Id,
                            ItemText = citem.Title,
                            Selected = false
                        });
                    }
                }
                Answers.Add(new FormAnswer { 
                    Id = ctrl.Id,
                    Title = ctrl.Title,
                    AnswerValue = 0,
                    AnswerText = "",
                    AnswerBoolValue = false,
                    ControlType = (int)ctrl.ControlType,
                    MaxValue = ctrl.MaxValue,
                    MaxMessage = ctrl.MaxMessage,
                    MinValue = ctrl.MinValue,
                    MinMessage = ctrl.MinMessage,
                    IsRequired = ctrl.IsRequired,
                    EmptyMessage = ctrl.EmptyMessage,
                    OrderId = ctrl.OrderId,
                    Items = items
                });
            }

            ViewBag.AnsObj = JsonConvert.SerializeObject(Answers);
            request.FormId = theForm.Id;
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Index(FormDataRequest request)
        {
            if (!ValidFormData(request))
            {
                var theForm = await iFormBuilderServ.iFormServ.FindAsync(x => x.Id == request.FormId);
                var controls = iFormBuilderServ.iFormControlServ.GetPart(x => x.FormId == request.FormId, 0, 30, x => x.OrderId, false);
                ViewBag.Form = theForm;
                List<FormAnswer> Answers = new List<FormAnswer>();

                foreach (var ctrl in controls)
                {
                    var items = new List<AnswerItem>();
                    if (ctrl.ControlType == ControlType.DropDown || ctrl.ControlType == ControlType.RadioList)
                    {
                        foreach (var citem in ctrl.ControlItems)
                        {
                            items.Add(new AnswerItem
                            {
                                ItemValue = citem.Id,
                                ItemText = citem.Title,
                                Selected = false
                            });
                        }
                    }
                    Answers.Add(new FormAnswer
                    {
                        Id = ctrl.Id,
                        Title = ctrl.Title,
                        AnswerValue = 0,
                        AnswerText = "",
                        AnswerBoolValue = false,
                        ControlType = (int)ctrl.ControlType,
                        MaxValue = ctrl.MaxValue,
                        MaxMessage = ctrl.MaxMessage,
                        MinValue = ctrl.MinValue,
                        MinMessage = ctrl.MinMessage,
                        IsRequired = ctrl.IsRequired,
                        EmptyMessage = ctrl.EmptyMessage,
                        OrderId = ctrl.OrderId,
                        Items = items
                    });
                }

                ViewBag.AnsObj = JsonConvert.SerializeObject(Answers);

                return View(request);
            }

            var item = new FormData();
            if (request.Id > 0)
            {
                item = await iFormBuilderServ.iFormDataServ.FindAsync(x => x.Id == request.Id);
            }

            item.AnswerObject = request.AnswerObject;

            if (request.Id == 0)
            {
                item.FormId = request.FormId;
                if (User.Identity.IsAuthenticated)
                {
                    var theUser = await userManager.GetUserAsync(HttpContext.User);
                    item.UserId = theUser.Id;
                }
                item.UserCookie = HttpContext.Request.Cookies[".AspNetCore.Identity.Application"];
                item.UserIP = HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();
                item.CreateDate = DateTime.Now;
                iFormBuilderServ.iFormDataServ.Add(item);
            }

            await iFormBuilderServ.iFormServ.SaveChangesAsync();

            return Redirect("/DynamicForms/?FormId=" + request.FormId);

        }

        private bool ValidFormData(FormDataRequest request)
        {
            if (request.FormId == 0)
                AddError("The title must have a value");

            if (string.IsNullOrEmpty(request.AnswerObject))
                AddError("Answer must have a value");

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                ViewBag.Messages = Messages;
                return false;
            }

            return true;
        }

    }
}
