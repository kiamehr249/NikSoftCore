using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.Bourse.Service;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.Bourse.Controllers.Panel
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin,Admin")]
    public class AdminBoard : NikController
    {
        public IBourseService iBourseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;

        public AdminBoard(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index(AdminContractSearch request)
        {
            ViewBag.PageTitle = "مدیریت قراردادها";
            var user = await userManager.GetUserAsync(HttpContext.User);
            var query = iBourseServ.iContractServ.ExpressionMaker();
            query.Add(x => true);
            bool isSearch = false;

            if (!string.IsNullOrEmpty(request.ContractNumber))
            {
                query.Add(x => x.ContractNumber.Contains(request.ContractNumber));
                isSearch = true;
            }

            if (!string.IsNullOrEmpty(request.UserFullName))
            {
                query.Add(x => x.UserFullName.Contains(request.UserFullName));
                isSearch = true;
            }

            if (request.ContractType > 0)
            {
                query.Add(x => x.ContractType == (ContractType)request.ContractType);
                isSearch = true;
            }

            if (request.Status > 0)
            {
                query.Add(x => x.Status == (ContractStatus)request.Status);
                isSearch = true;
            }

            if (request.BranchId > 0)
            {
                query.Add(x => x.BranchId == request.BranchId);
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iContractServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;
            ViewBag.Contents = iBourseServ.iContractServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();

            BranchBinder(request.BranchId);
            return View(request);
        }


        private void BranchBinder(int branchId)
        {
            var branches = iBourseServ.iBranchServ.GetAll(x => true, y => new { y.Id, y.Title });
            ViewBag.Branches = new SelectList(branches, "Id", "Title", branchId);
        }


        public async Task<IActionResult> Accept(int Id)
        {
            var item = await iBourseServ.iContractServ.FindAsync(x => x.Id == Id);
            item.Status = ContractStatus.Accept;
            await iBourseServ.iContractServ.SaveChangesAsync();
            return Redirect("/Panel/AdminBoard");
        }

        public async Task<IActionResult> Ignore(int Id)
        {
            var item = await iBourseServ.iContractServ.FindAsync(x => x.Id == Id);
            item.Status = ContractStatus.Ignore;
            await iBourseServ.iContractServ.SaveChangesAsync();
            return Redirect("/Panel/AdminBoard");
        }

        public async Task<IActionResult> InProgress(int Id)
        {
            var item = await iBourseServ.iContractServ.FindAsync(x => x.Id == Id);
            item.Status = ContractStatus.InProccess;
            await iBourseServ.iContractServ.SaveChangesAsync();
            return Redirect("/Panel/AdminBoard");
        }

    }
}
