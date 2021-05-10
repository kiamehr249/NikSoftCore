using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using NiksoftCore.ViewModel.User;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.SystemBase.Controllers.Panel.Modules
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin")]
    public class RoleManage : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly RoleManager<DataModel.Role> roleManager;

        public RoleManage(IConfiguration Configuration,
            UserManager<DataModel.User> userManager,
            RoleManager<DataModel.Role> roleManager) : base(Configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IActionResult Index(BaseRequest request)
        {
            ViewBag.PageTitle = "مدیریت نقش ها";

            var query = ISystemBaseServ.iNikRoleServ.ExpressionMaker();
            query.Add(x => true);
            var total = ISystemBaseServ.iNikRoleServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;

            ViewBag.Roles = ISystemBaseServ.iNikRoleServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {

            ViewBag.PageTitle = "ایجاد نقش";

            var request = new RoleRequest();
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                AddError("نام نقش کاربری باید مقدار داشته باشد", "fa");

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            await roleManager.CreateAsync(new DataModel.Role
            {
                Name = request.Name
            });

            return Redirect("/Panel/RoleManage");

        }


        [HttpGet]
        public IActionResult Edit(int Id)
        {

            var theRole = roleManager.Roles.First(x => x.Id == Id);
            var request = new RoleRequest
            {
                Id = theRole.Id,
                Name = theRole.Name,
                NormalizedName = theRole.NormalizedName
            };
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                AddError("نام نقش کاربری باید مقدار داشته باشد", "fa");

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            var theRole = roleManager.Roles.First(x => x.Id == request.Id);
            theRole.Name = request.Name;
            await roleManager.UpdateAsync(theRole);

            return Redirect("/Panel/RoleManage");
        }


        public async Task<IActionResult> Remove(int Id)
        {
            var theRole = roleManager.Roles.First(x => x.Id == Id);
            await roleManager.DeleteAsync(theRole);
            return Redirect("/Panel/RoleManage");
        }
    }
}
