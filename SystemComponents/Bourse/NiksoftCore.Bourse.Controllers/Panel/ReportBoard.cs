using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.Bourse.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.Bourse.Controllers.Panel
{
    [Area("Panel")]
    [Authorize]
    public class ReportBoard : NikController
    {
        public IBourseService iBourseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;

        public ReportBoard(IConfiguration Config, UserManager<DataModel.User> userManager) : base(Config)
        {
            this.iBourseServ = new BourseService(Config.GetConnectionString("SystemBase"));
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(FinancialSearch request)
        {
            ViewBag.PageTitle = "گزارش تراکنش ها";
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

        [HttpGet]
        public async Task<IActionResult> MarketerReport(MarketerReportSearch request)
        {
            ViewBag.PageTitle = "خلاصه عمل کرد - بازاریاب شعبه";
            var user = await userManager.GetUserAsync(HttpContext.User);

            var isMaster = await userManager.IsInRoleAsync(user, "Master");

            var query = iBourseServ.iBaseTransactionServ.ExpressionMaker();

            if (request.Start == 0)
            {
                request.Start = PersianDateTime.Now.Year * 100;
            }

            if (request.End == 0)
            {
                request.End = (PersianDateTime.Now.Year * 100) + PersianDateTime.Now.Month;
            }

            var query2 = iBourseServ.iBranchServ.ExpressionMaker();
            query2.Add(x => true);

            if (isMaster)
            {
                var bids = iBourseServ.iBranchMasterServ.GetAll(x => x.UserId == user.Id, y => new { y.BranchId }).Select(x => x.BranchId).ToList();
                query2.Add(x => bids.Contains(x.Id));
            }

            var branches = iBourseServ.iBranchServ.GetAll(query2, y => new { y.Code, y.Title }).ToList();

            if (!isMaster)
            {
                branches.Insert(0, new
                {
                    Code = "0",
                    Title = "همه شعب"
                });

                if (!string.IsNullOrEmpty(request.BranchCode) && request.BranchCode != "0")
                {
                    query.Add(x => x.BranchCode == request.BranchCode);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(request.BranchCode) || request.BranchCode == "0")
                {
                    request.BranchCode = branches.First().Code;
                }
                query.Add(x => x.BranchCode == request.BranchCode);
            }

            ViewBag.Branches = new SelectList(branches, "Code", "Title", request.BranchCode);

            query.Add(x => x.Period >= request.Start && x.Period <= request.End);

            ViewBag.Contents = iBourseServ.iBaseTransactionServ.GetAll(query).ToList();
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> ConsultantReport(ReportSearch request)
        {
            ViewBag.PageTitle = "خلاصه عمل کرد - مشاور بازاریاب شعبه";
            var user = await userManager.GetUserAsync(HttpContext.User);
            var theProfile = await iBourseServ.iUserProfileServ.FindAsync(x => x.UserId == user.Id);
            var theMarketer = await iBourseServ.iBranchMarketerServ.FindAsync(x => x.UserId == user.Id);
            request.Code = theProfile.UserCode;

            if (request.Start == 0)
            {
                request.Start = PersianDateTime.Now.Year * 100;
            }

            if (request.End == 0)
            {
                request.End = (PersianDateTime.Now.Year * 100) + PersianDateTime.Now.Month;
            }

            ViewBag.Contents = iBourseServ.GetConsultantReport(theProfile.UserCode, request.Start, request.End);
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> ConsultantDetailReport(ReportSearch request)
        {
            ViewBag.PageTitle = "جزئیات عملکرد - مشاور بازاریابی";
            var user = await userManager.GetUserAsync(HttpContext.User);
            var theProfile = await iBourseServ.iUserProfileServ.FindAsync(x => x.UserId == user.Id);

            var query = iBourseServ.iBaseTransactionServ.ExpressionMaker();


            if (request.Start == 0)
            {
                request.Start = PersianDateTime.Now.Year * 100;
            }

            if (request.End == 0)
            {
                request.End = (PersianDateTime.Now.Year * 100) + PersianDateTime.Now.Month;
            }

            query.Add(x => x.MarketerCode == theProfile.UserCode && x.Period <= request.End && x.Period >= request.Start);
            ViewBag.Contents = iBourseServ.iBaseTransactionServ.GetAll(query).ToList();

            return View(request);
        }


        private void BranchBinder(string branchCode)
        {
            var branches = iBourseServ.iBranchServ.GetAll(x => true, y => new { y.Code, y.Title });
            ViewBag.Branches = new SelectList(branches, "Code", "Title", branchCode);
        }


    }
}
