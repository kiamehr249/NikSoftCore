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
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.Bourse.Controllers.Panel
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin,Admin,Master,Marketer")]
    public class FinancialBoard : NikController
    {
        public IBourseService iBourseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;

        public FinancialBoard(IConfiguration Config, UserManager<DataModel.User> userManager) : base(Config)
        {
            this.iBourseServ = new BourseService(Config.GetConnectionString("SystemBase"));
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(FinancialSearch request)
        {
            ViewBag.PageTitle = "مدیریت مالی";
            var user = await userManager.GetUserAsync(HttpContext.User);
            var query = iBourseServ.iBaseTransactionServ.ExpressionMaker();
            query.Add(x => true);
            bool isSearch = false;

            if (request.Period != 0)
            {
                query.Add(x => x.Period == request.Period);
                isSearch = true;
            }

            if (!string.IsNullOrEmpty(request.BranchCode))
            {
                query.Add(x => x.BranchCode.Contains(request.BranchCode));
                isSearch = true;
            }

            if (!string.IsNullOrEmpty(request.ConsultantCode))
            {
                query.Add(x => x.ConsultantCode.Contains(request.ConsultantCode));
                isSearch = true;
            }

            if (!string.IsNullOrEmpty(request.ConsultantName))
            {
                query.Add(x => x.ConsultantName.Contains(request.ConsultantName));
                isSearch = true;
            }


            ViewBag.Search = isSearch;

            var total = iBourseServ.iBaseTransactionServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;
            ViewBag.Contents = iBourseServ.iBaseTransactionServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            BranchBinder(request.BranchCode);
            return View(request);
        }

        public async Task<IActionResult> Receipts(ReceiptSearch request)
        {
            ViewBag.PageTitle = "مدیریت سند مالی";

            var user = await userManager.GetUserAsync(HttpContext.User);
            ViewBag.Transaction = await iBourseServ.iBaseTransactionServ.FindAsync(x => x.Id == request.TransactionId);
            var query = iBourseServ.iPaymentReceiptServ.ExpressionMaker();
            query.Add(x => x.TransactionId == request.TransactionId);

            bool isSearch = false;

            if (!string.IsNullOrEmpty(request.Number))
            {
                query.Add(x => x.Number.Contains(request.Number));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iPaymentReceiptServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;
            ViewBag.Contents = iBourseServ.iPaymentReceiptServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> CreateReceipt(int TransactionId, int Id)
        {
            var theTrans = await iBourseServ.iBaseTransactionServ.FindAsync(x => x.Id == TransactionId);
            ViewBag.Transaction = theTrans;
            var request = new ReceiptRequest();

            var theConProfile = await iBourseServ.iUserProfileServ.FindAsync(x => x.UserCode == theTrans.ConsultantCode);
            if (theConProfile == null)
            {
                AddError("کاربر مشاور بازاریابی با این کد یافت نشد.");
                ViewBag.Messages = Messages;
                return View(request);
            }
            var theBankAccount = await iBourseServ.iUserBankAccountServ.FindAsync(x => x.UserId == theConProfile.UserId);
            ViewBag.PageTitle = "ایجاد سند جدید";

            
            if (Id > 0)
            {
                var item = iBourseServ.iPaymentReceiptServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Number = item.Number;
                request.ReceiptDate = item.ReceiptDate.ToPersianDateTime().ToString(PersianDateTimeFormat.Date);
                request.Period = item.Period;
                request.PaymentAmount = item.PaymentAmount;
                request.Description = item.Description;
                request.TrackingCode = item.TrackingCode;
                request.Status = (int)item.Status;
            }

            
            request.Period = theTrans.Period;
            request.PAN = theBankAccount.PAN;
            request.UserId = theConProfile.UserId;
            request.TransactionId = TransactionId;
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReceipt(ReceiptRequest request)
        {
            ViewBag.PageTitle = "ایجاد سند جدید";

            BaseTransaction theTransaction = iBourseServ.iBaseTransactionServ.Find(x => x.Id == request.TransactionId);
            ViewBag.Transaction = theTransaction;

            if (request.Id == 0 && (theTransaction.ConsultantWage - theTransaction.PaymentAmount) < request.PaymentAmount)
            {
                ViewBag.Messages = Messages;
                AddError("مبلغ پرداختی از مبلغ مانده حساب بیشتر می باشد.");
                return View(request);
            }
            
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (!ValidReceiptForm(request))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            PaymentReceipt item;
            if (request.Id > 0)
            {
                item = iBourseServ.iPaymentReceiptServ.Find(x => x.Id == request.Id);

                if ((theTransaction.ConsultantWage - theTransaction.PaymentAmount) < (request.PaymentAmount - item.PaymentAmount))
                {
                    ViewBag.Messages = Messages;
                    AddError("مبلغ پرداختی از مبلغ مانده حساب بیشتر می باشد.");
                    return View(request);
                }

                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
                theTransaction.PaymentAmount += (request.PaymentAmount - item.PaymentAmount);
            }
            else
            {
                item = new PaymentReceipt();
                item.CreateDate = DateTime.Now;
                item.CreatedBy = user.Id;
            }

            item.Number = request.Number;
            item.ReceiptDate = PersianDateTime.Parse(request.ReceiptDate).ToDateTime(); ;
            item.Period = request.Period;
            item.PaymentAmount = request.PaymentAmount;
            item.UserId = request.UserId;
            item.PAN = request.PAN;
            item.Description = request.Description;
            item.TransactionId = request.TransactionId;
            item.Status = ReceiptStatus.Save;
            item.TrackingCode = "";

            if (request.Id == 0)
            {
                theTransaction.PaymentAmount += request.PaymentAmount;
                iBourseServ.iPaymentReceiptServ.Add(item);
            }

            await iBourseServ.iPaymentReceiptServ.SaveChangesAsync();
            item.TrackingCode = NikTools.RandomString(5, item.Id.ToString());
            await iBourseServ.iPaymentReceiptServ.SaveChangesAsync();
            await iBourseServ.iBaseTransactionServ.SaveChangesAsync();
            
            return Redirect("/Panel/FinancialBoard/Receipts/?TransactionId=" + request.TransactionId);

        }

        public async Task<IActionResult> RemoveReceipt(int Id)
        {
            var item = await iBourseServ.iPaymentReceiptServ.FindAsync(x => x.Id == Id);
            var theTransaction = await iBourseServ.iBaseTransactionServ.FindAsync(x => x.Id == item.TransactionId);
            theTransaction.PaymentAmount = theTransaction.PaymentAmount - item.PaymentAmount;
            iBourseServ.iPaymentReceiptServ.Remove(item);
            await iBourseServ.iPaymentReceiptServ.SaveChangesAsync();
            await iBourseServ.iBaseTransactionServ.SaveChangesAsync();
            return Redirect("/Panel/FinancialBoard/Receipts/?TransactionId=" + theTransaction.Id);
        }


        private void BranchBinder(string BranchCode)
        {
            var branches = iBourseServ.iBranchServ.GetAll(x => true, y => new { y.Code, y.Title });
            ViewBag.Branches = new SelectList(branches, "Code", "Title", BranchCode);
        }

        private bool ValidReceiptForm(ReceiptRequest request)
        {
            if (string.IsNullOrEmpty(request.Number))
            {
                AddError("شماره سند باید مقدار داشته باشد", "fa");
            }

            if (string.IsNullOrEmpty(request.ReceiptDate))
            {
                AddError("تاریخ سند باید مقدار داشته باشد", "fa");
            }

            if (request.PaymentAmount == 0)
            {
                AddError("مبلغ پرداختی باید مقدار داشته باشد", "fa");
            }

            if (request.Period == 0)
            {
                AddError("دوره باید مقدار داشته باشد", "fa");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                return false;
            }

            return true;

        }


    }
}
