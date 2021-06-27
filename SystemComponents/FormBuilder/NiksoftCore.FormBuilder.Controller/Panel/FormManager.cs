using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.FormBuilder.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.FormBuilder.Controller.Panel
{
    [Area("Panel")]
    [Authorize]
    public class FormManager : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly IWebHostEnvironment hosting;
        private IFormBuilderService iFormBuilderServ { get; set; }

        public FormManager(IConfiguration Configuration, IWebHostEnvironment hostingEnvironment,
            UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.userManager = userManager;
            hosting = hostingEnvironment;
            iFormBuilderServ = new FormBuilderService(Configuration.GetConnectionString("SystemBase"));
        }

        #region Form Manager
        public IActionResult Index(FormSearch request)
        {
            ViewBag.PageTitle = "Forms Management";

            var query = iFormBuilderServ.iFormServ.ExpressionMaker();
            query.Add(x => true);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            ViewBag.Search = isSearch;


            var total = iFormBuilderServ.iFormServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iFormBuilderServ.iFormServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View(request);
        }

        [HttpGet]
        public IActionResult Create(int Id)
        {

            ViewBag.PageTitle = "Create Form";

            var request = new FormRequest();
            if (Id > 0)
            {
                var item = iFormBuilderServ.iFormServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.Description = item.Description;
                request.Message = item.Message;
                request.LoginRequired = item.LoginRequired;
                request.Roles = item.Roles;
            }
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(FormRequest request)
        {
            if (!ValidForm(request))
            {
                return View(request);
            }

            var item = new Form();
            if (request.Id > 0)
            {
                item = await iFormBuilderServ.iFormServ.FindAsync(x => x.Id == request.Id);
            }

            item.Title = request.Title;
            item.Description = request.Description;
            item.Message = request.Message;
            item.Roles = request.Roles;

            if (request.Id == 0)
            {
                iFormBuilderServ.iFormServ.Add(item);
            }

            await iFormBuilderServ.iFormServ.SaveChangesAsync();

            return Redirect("/Panel/FormManager");

        }

        private bool ValidForm(FormRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
                AddError("The title must have a value");

            if(string.IsNullOrEmpty(request.Message))
                AddError("The message must have a value");

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                ViewBag.Messages = Messages;
                return false;
            }

            return true;
        }


        public async Task<IActionResult> Remove(int Id)
        {
            var item = await iFormBuilderServ.iFormServ.FindAsync(x => x.Id == Id);
            iFormBuilderServ.iFormServ.Remove(item);
            await iFormBuilderServ.iFormServ.SaveChangesAsync();
            return Redirect("/Panel/FormManager");
        }

        public async Task<IActionResult> LoginRequired(int Id)
        {
            var item = await iFormBuilderServ.iFormServ.FindAsync(x => x.Id == Id);
            item.LoginRequired = !item.LoginRequired;
            await iFormBuilderServ.iFormServ.SaveChangesAsync();
            return Redirect("/Panel/FormManager");
        }
        #endregion

        #region Form Controls
        public IActionResult Controls(ControlSearch request)
        {
            ViewBag.PageTitle = "Form Controls";

            var query = iFormBuilderServ.iFormControlServ.ExpressionMaker();
            query.Add(x => x.FormId == request.FormId);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iFormBuilderServ.iFormControlServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iFormBuilderServ.iFormControlServ.GetPart(query, pager.StartIndex, pager.PageSize, x => x.OrderId, false).ToList();
            return View(request);
        }

        [HttpGet]
        public IActionResult CreateControl(int FormId, int Id)
        {

            ViewBag.PageTitle = "Create Form";

            var request = new ControlRequest();
            if (Id > 0)
            {
                var item = iFormBuilderServ.iFormControlServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.ControlType = (int)item.ControlType;
                request.MaxValue = item.MaxValue;
                request.MaxMessage = item.MaxMessage;
                request.MinValue = item.MinValue;
                request.MinMessage = item.MinMessage;
                request.IsRequired = item.IsRequired;
                request.EmptyMessage = item.EmptyMessage;
            }

            request.FormId = FormId;
            DDLTypeBinder(request.ControlType);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> CreateControl(ControlRequest request)
        {
            if (!ValidControl(request))
            {
                DDLTypeBinder(request.ControlType);
                return View(request);
            }

            var item = new FormControl();
            if (request.Id > 0)
            {
                item = await iFormBuilderServ.iFormControlServ.FindAsync(x => x.Id == request.Id);
            }

            item.Title = request.Title;
            item.ControlType = (ControlType)request.ControlType;
            item.MaxValue = request.MaxValue;
            item.MaxMessage = request.MaxMessage;
            item.MinValue = request.MinValue;
            item.MinMessage = request.MinMessage;

            if (request.Id == 0)
            {
                var maxOrder = iFormBuilderServ.iFormControlServ.Count(x => x.FormId == request.FormId);
                item.OrderId = maxOrder + 1;
                item.FormId = request.FormId;
                iFormBuilderServ.iFormControlServ.Add(item);
            }

            await iFormBuilderServ.iFormControlServ.SaveChangesAsync();

            return Redirect("/Panel/FormManager/Controls/?FormId=" + request.FormId);

        }

        private bool ValidControl(ControlRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
                AddError("The title must have a value");

            if (request.ControlType == 0)
                AddError("The control type must have a value");

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                ViewBag.Messages = Messages;
                return false;
            }

            return true;
        }


        public async Task<IActionResult> RemoveControl(int Id)
        {
            var item = await iFormBuilderServ.iFormControlServ.FindAsync(x => x.Id == Id);
            var formid = item.FormId;
            iFormBuilderServ.iFormControlServ.Remove(item);
            await iFormBuilderServ.iFormControlServ.SaveChangesAsync();
            return Redirect("/Panel/FormManager/Controls/?FormId=" + formid);
        }

        [Authorize(Roles = "NikAdmin,Admin")]
        public async Task<IActionResult> OrderUpControl(int Id)
        {
            var theItem = iFormBuilderServ.iFormControlServ.Find(x => x.Id == Id);

            if (theItem.OrderId == 1)
            {
                return Redirect("/Panel/FormManager/Controls/?FormId=" + theItem.FormId);
            }

            var upItem = iFormBuilderServ.iFormControlServ.Find(x => x.FormId == theItem.FormId && x.OrderId == (theItem.OrderId - 1));

            theItem.OrderId = theItem.OrderId - 1;
            upItem.OrderId = upItem.OrderId + 1;

            await iFormBuilderServ.iFormControlServ.SaveChangesAsync();
            return Redirect("/Panel/FormManager/Controls/?FormId=" + theItem.FormId);
        }

        [Authorize(Roles = "NikAdmin,Admin")]
        public async Task<IActionResult> OrderDownControl(int Id)
        {
            var theItem = iFormBuilderServ.iFormControlServ.Find(x => x.Id == Id);
            var itemsCount = iFormBuilderServ.iFormControlServ.Count(x => x.FormId == theItem.FormId);
            if (theItem.OrderId == itemsCount)
            {
                return Redirect("/Panel/FormManager/Controls/?FormId=" + theItem.FormId);
            }

            var upItem = iFormBuilderServ.iFormControlServ.Find(x => x.FormId == theItem.FormId && x.OrderId == (theItem.OrderId + 1));

            theItem.OrderId = theItem.OrderId + 1;
            upItem.OrderId = upItem.OrderId - 1;

            await iFormBuilderServ.iFormControlServ.SaveChangesAsync();
            return Redirect("/Panel/FormManager/Controls/?FormId=" + theItem.FormId);
        }

        private void DDLTypeBinder(int ctype)
        {
            List<ListItemModel> types = new List<ListItemModel>();
            types.Add(new ListItemModel
            {
                Id = 1,
                Title = "Text Box"
            });
            types.Add(new ListItemModel
            {
                Id = 2,
                Title = "Text Area"
            });
            types.Add(new ListItemModel
            {
                Id = 3,
                Title = "Text Editor"
            });
            types.Add(new ListItemModel
            {
                Id = 4,
                Title = "Check Box"
            });
            types.Add(new ListItemModel
            {
                Id = 5,
                Title = "Drop Down List"
            });
            types.Add(new ListItemModel
            {
                Id = 6,
                Title = "Radio Button List"
            });
            types.Add(new ListItemModel
            {
                Id = 7,
                Title = "File Upload"
            });
            ViewBag.ControlTypes = new SelectList(types, "Id", "Title", ctype);
        }

        #endregion

        #region Control Items
        public IActionResult ControlItems(ControlItemSearch request)
        {
            
            var theControl = iFormBuilderServ.iFormControlServ.Find(x => x.Id == request.ControlId);
            ViewBag.Control = theControl;
            ViewBag.PageTitle = "Control Items";

            var query = iFormBuilderServ.iControlItemServ.ExpressionMaker();
            query.Add(x => x.ControlId == request.ControlId);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iFormBuilderServ.iControlItemServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iFormBuilderServ.iControlItemServ.GetPart(query, pager.StartIndex, pager.PageSize, x => x.OrderId, false).ToList();
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> CreateItem(int ControlId, int Id)
        {
            var theControl = await iFormBuilderServ.iFormControlServ.FindAsync(x => x.Id == ControlId);
            ViewBag.PageTitle = "Create Control Item";

            var request = new ControlItemRequest();
            if (Id > 0)
            {
                var item = await iFormBuilderServ.iControlItemServ.FindAsync(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
            }

            request.FormId = theControl.FormId;
            request.ControlId = theControl.Id;
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(ControlItemRequest request)
        {
            ViewBag.PageTitle = "Create Control Item";

            if (!ValidControlItem(request))
            {
                return View(request);
            }

            var item = new ControlItem();
            if (request.Id > 0)
            {
                item = await iFormBuilderServ.iControlItemServ.FindAsync(x => x.Id == request.Id);
            }

            item.Title = request.Title;

            if (request.Id == 0)
            {
                var maxOrder = iFormBuilderServ.iControlItemServ.Count(x => x.ControlId == request.ControlId);
                item.OrderId = maxOrder + 1;
                item.FormId = request.FormId;
                item.ControlId = request.ControlId;
                iFormBuilderServ.iControlItemServ.Add(item);
            }

            await iFormBuilderServ.iControlItemServ.SaveChangesAsync();

            return Redirect("/Panel/FormManager/ControlItems/?ControlId=" + request.ControlId);

        }

        private bool ValidControlItem(ControlItemRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
                AddError("The title must have a value");

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                ViewBag.Messages = Messages;
                return false;
            }

            return true;
        }


        public async Task<IActionResult> RemoveControlItem(int Id)
        {
            var item = await iFormBuilderServ.iControlItemServ.FindAsync(x => x.Id == Id);
            var controlId = item.ControlId;
            iFormBuilderServ.iControlItemServ.Remove(item);
            await iFormBuilderServ.iControlItemServ.SaveChangesAsync();
            return Redirect("/Panel/FormManager/ControlItems/?ControlId=" + controlId);
        }

        [Authorize(Roles = "NikAdmin,Admin")]
        public async Task<IActionResult> OrderUpControlItem(int Id)
        {
            var theItem = iFormBuilderServ.iControlItemServ.Find(x => x.Id == Id);

            if (theItem.OrderId == 1)
            {
                return Redirect("/Panel/FormManager/ControlItems/?ControlId=" + theItem.ControlId);
            }

            var upItem = iFormBuilderServ.iControlItemServ.Find(x => x.ControlId == theItem.ControlId && x.OrderId == (theItem.OrderId - 1));

            theItem.OrderId = theItem.OrderId - 1;
            upItem.OrderId = upItem.OrderId + 1;

            await iFormBuilderServ.iControlItemServ.SaveChangesAsync();
            return Redirect("/Panel/FormManager/ControlItems/?ControlId=" + theItem.ControlId);
        }

        [Authorize(Roles = "NikAdmin,Admin")]
        public async Task<IActionResult> OrderDownControlItem(int Id)
        {
            var theItem = iFormBuilderServ.iControlItemServ.Find(x => x.Id == Id);
            var itemsCount = iFormBuilderServ.iControlItemServ.Count(x => x.ControlId == theItem.ControlId);
            if (theItem.OrderId == itemsCount)
            {
                return Redirect("/Panel/FormManager/ControlItems/?ControlId=" + theItem.ControlId);
            }

            var upItem = iFormBuilderServ.iControlItemServ.Find(x => x.ControlId == theItem.ControlId && x.OrderId == (theItem.OrderId + 1));

            theItem.OrderId = theItem.OrderId + 1;
            upItem.OrderId = upItem.OrderId - 1;

            await iFormBuilderServ.iControlItemServ.SaveChangesAsync();
            return Redirect("/Panel/FormManager/ControlItems/?ControlId=" + theItem.ControlId);
        }

        #endregion


    }
}
