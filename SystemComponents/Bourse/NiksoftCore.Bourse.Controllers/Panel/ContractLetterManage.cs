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
    public class ContractLetterManage : NikController
    {
        public IBourseService iBourseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;

        public ContractLetterManage(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
            this.userManager = userManager;
        }

        public IActionResult Index(ContractLetterSearch request)
        {
            ViewBag.PageTitle = "مدیریت متن قراردادها";

            var query = iBourseServ.iContractLetterServ.ExpressionMaker();
            query.Add(x => true);
            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            if (request.ContractType > 0)
            {
                query.Add(x => x.ContractType == (ContractType)request.ContractType);
                isSearch = true;
            }

            if (request.Year > 0)
            {
                query.Add(x => x.Year == request.Year);
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iContractLetterServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;
            ComboBinder(request.ContractType, request.Year);
            ViewBag.Contents = iBourseServ.iContractLetterServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View(request);
        }

        [HttpGet]
        public IActionResult Create(int Id)
        {
            ViewBag.PageTitle = "ایجاد متن جدید";

            var request = new ContractLetterRequest();
            if (Id > 0)
            {
                var item = iBourseServ.iContractLetterServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.ContractType = (int)item.ContractType;
                request.FullText = item.FullText;
                request.Year = item.Year;
                request.CurrentDate = item.CurrentDate != null ? item.CurrentDate.Value.ToPersianDateTime().ToString(PersianDateTimeFormat.Date) : "";
                request.Enabled = item.Enabled;
            }
            ComboBinder(request.ContractType, request.Year);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContractLetterRequest request)
        {
            ViewBag.Messages = Messages;
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (!ValidForm(request))
            {
                ComboBinder(request.ContractType, request.Year);
                return View(request);
            }

            ContractLetter item;
            if (request.Id > 0)
            {
                item = iBourseServ.iContractLetterServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }
            else
            {
                item = new ContractLetter();
                item.CreateDate = DateTime.Now;
                item.CreatedBy = user.Id;
                item.Enabled = false;
            }

            item.Title = request.Title;
            item.FullText = request.FullText;
            item.ContractType = (ContractType)request.ContractType;
            item.Year = request.Year;
            item.CurrentDate = PersianDateTime.Parse(request.CurrentDate).ToDateTime();

            if (request.Id == 0)
            {
                iBourseServ.iContractLetterServ.Add(item);
            }

            await iBourseServ.iContractLetterServ.SaveChangesAsync();
            return Redirect("/Panel/ContractLetterManage");

        }

        public async Task<IActionResult> Remove(int Id)
        {
            var item = await iBourseServ.iContractLetterServ.FindAsync(x => x.Id == Id);
            iBourseServ.iContractLetterServ.Remove(item);
            await iBourseServ.iContractLetterServ.SaveChangesAsync();
            return Redirect("/Panel/ContractLetterManage");
        }

        public async Task<IActionResult> Enable(int Id)
        {
            var item = await iBourseServ.iContractLetterServ.FindAsync(x => x.Id == Id);
            var otherItem = iBourseServ.iContractLetterServ.Find(x => x.Year == item.Year && x.Id != item.Id && x.ContractType == item.ContractType);
            if (otherItem != null)
            {
                otherItem.Enabled = false;
            }

            item.Enabled = !item.Enabled;
            await iBourseServ.iContractLetterServ.SaveChangesAsync();
            return Redirect("/Panel/ContractLetterManage");
        }

        private bool ValidForm(ContractLetterRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("عنوان باید مقدار داشته باشد", "fa");
            }

            if (request.ContractType == 0)
            {
                AddError("نوع قرارداد باید مقدار داشته باشد", "fa");
            }

            if (request.Year == 0)
            {
                AddError("سال باید مقدار داشته باشد", "fa");
            }

            if (string.IsNullOrEmpty(request.FullText))
            {
                AddError("متن باید مقدار داشته باشد", "fa");
            }

            if (string.IsNullOrEmpty(request.CurrentDate))
            {
                AddError("تاریخ متن باید مقدار داشته باشد", "fa");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                return false;
            }

            return true;

        }

        private void ComboBinder(int coType, int year)
        {
            List<ListItemModel> cTypes = new List<ListItemModel>();
            cTypes.Add(new ListItemModel
            {
                Id = 0,
                Title = "انتخاب کنید"
            });

            cTypes.Add(new ListItemModel
            {
                Id = 1,
                Title = "بازاریاب شعبه"
            });
            cTypes.Add(new ListItemModel
            {
                Id = 2,
                Title = "مشاور بازاریابی"
            });
            ViewBag.ContractTypes = new SelectList(cTypes, "Id", "Title", coType);

            var pdate = new PersianDateTime(DateTime.Now);
            List<ListItemModel> years = new List<ListItemModel>();

            var start = pdate.Year - 5;
            var end = pdate.Year + 5;

            years.Add(new ListItemModel
            {
                Id = 0,
                Title = "انتخاب کنید"
            });

            for (var i = start; i <= end; i++)
            {
                years.Add(new ListItemModel {
                    Id = i,
                    Title = i.ToString()
                });
            }
            ViewBag.Years = new SelectList(years, "Id", "Title", year);
        }

    }
}
