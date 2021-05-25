using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.Bourse.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.Bourse.Controllers.Panel
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin,Admin")]
    public class FeeManage : NikController
    {
        public IBourseService iBourseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;

        public FeeManage(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
            this.userManager = userManager;
        }

        public IActionResult Index(FeeSearch request)
        {
            ViewBag.PageTitle = "مدیریت نرخ کارمزد";

            var query = iBourseServ.iFeeServ.ExpressionMaker();
            query.Add(x => true);
            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            if (request.FeeType > 0)
            {
                query.Add(x => x.FeeType == (FeeType)request.FeeType);
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iFeeServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iBourseServ.iFeeServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View();
        }

        [HttpGet]
        public IActionResult Create(int Id)
        {

            ViewBag.PageTitle = "ایجاد نرخ جدید";

            var request = new FeeRequest();
            if (Id > 0)
            {
                var item = iBourseServ.iFeeServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.FeeType = (int)item.FeeType;
                request.FromAmount = item.FromAmount;
                request.ToAmount = item.ToAmount;
                request.AmountPercentage = item.AmountPercentage;
            }
            FeeTypeBinder(request.FeeType);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(FeeRequest request)
        {
            ViewBag.Messages = Messages;
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (!ValidForm(request))
            {
                FeeTypeBinder(request.FeeType);
                return View(request);
            }

            Fee item;
            if (request.Id > 0)
            {
                item = iBourseServ.iFeeServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }
            else
            {
                item = new Fee();
                item.CreateDate = DateTime.Now;
                item.CreatedBy = user.Id;
            }

            item.Title = request.Title;
            item.FeeType = (FeeType)request.FeeType;
            item.FromAmount = request.FromAmount;
            item.ToAmount = request.ToAmount;
            item.AmountPercentage = request.AmountPercentage;

            if (request.Id == 0)
            {
                item.CreateDate = DateTime.Now;
                item.CreatedBy = user.Id;
                iBourseServ.iFeeServ.Add(item);
            }
            else
            {
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }

            await iBourseServ.iFeeServ.SaveChangesAsync();
            return Redirect("/Panel/FeeManage");

        }

        public async Task<IActionResult> Remove(int Id)
        {
            var item = await iBourseServ.iFeeServ.FindAsync(x => x.Id == Id);
            iBourseServ.iFeeServ.Remove(item);
            await iBourseServ.iFeeServ.SaveChangesAsync();
            return Redirect("/Panel/FeeManage");
        }

        private bool ValidForm(FeeRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("عنوان نرخ باید مقدار داشته باشد", "fa");
            }

            if (request.FeeType == 0)
            {
                AddError("نوع نرخ باید مقدار داشته باشد", "fa");
            }

            if (request.FeeType == 2 && request.FromAmount == 0)
            {
                AddError("از مبلغ باید مقدار داشته باشد", "fa");
            }

            if (request.FeeType == 2 && request.ToAmount == 0)
            {
                AddError("تا مبلغ باید مقدار داشته باشد", "fa");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                return false;
            }

            return true;

        }

        private void FeeTypeBinder(int feeType)
        {
            List<ListItemModel> feeTypes = new List<ListItemModel>();
            feeTypes.Add(new ListItemModel
            {
                Id = 1,
                Title = "ثابت"
            });
            feeTypes.Add(new ListItemModel
            {
                Id = 2,
                Title = "پلکانی"
            });
            ViewBag.FeeTypes = new SelectList(feeTypes, "Id", "Title", feeType);
        }

    }
}
