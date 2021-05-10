using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.SystemBase.Controllers.Panel.Modules
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin")]
    public class UserRoleManage : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly RoleManager<DataModel.Role> roleManager;

        public UserRoleManage(IConfiguration Configuration,
            UserManager<DataModel.User> userManager,
            RoleManager<DataModel.Role> roleManager) : base(Configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IActionResult Index(int Id)
        {
            ViewBag.PageTitle = "مدیریت نقش های کاربر";
            ViewBag.User = userManager.Users.Where(x => x.Id == Id).ToList().First();
            ViewBag.Profile = ISystemBaseServ.iUserProfileServ.Find(x => x.UserId == Id);
            //ViewBag.UserRoles = roleManager.Use.Where(x => x.)

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
            {
                AddError("نام نقش کاربری باید مقدار داشته باشد", "fa");

            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            await roleManager.CreateAsync(new DataModel.Role
            {
                Name = request.Name
            });

            return Redirect("/Panel/UserRoleManage");

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

            if (request.Id < 1)
            {
                AddError("خطا در ویرایش لطفا از ابتدا عملیات را انجام دهید", "fa");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            var theRole = roleManager.Roles.First(x => x.Id == request.Id);
            theRole.Name = request.Name;
            await roleManager.UpdateAsync(theRole);

            return Redirect("/Panel/UserRoleManage");
        }


        public async Task<IActionResult> Remove(int Id)
        {
            var theRole = roleManager.Roles.First(x => x.Id == Id);
            await roleManager.DeleteAsync(theRole);
            return Redirect("/Panel/UserRoleManage");
        }


    }
}
