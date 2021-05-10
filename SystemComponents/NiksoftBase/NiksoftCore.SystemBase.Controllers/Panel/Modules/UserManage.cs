using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.SystemBase.Service;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.SystemBase.Controllers.Panel.Modules
{
    [Area("Panel")]
    public class UserManage : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly RoleManager<DataModel.Role> roleManager;

        public UserManage(IConfiguration Configuration,
            UserManager<DataModel.User> userManager,
            RoleManager<DataModel.Role> roleManager) : base(Configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IActionResult Index(UserGridRequest request)
        {
            var query = ISystemBaseServ.iNikUserServ.ExpressionMaker();
            query.Add(x => true);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.UserName))
            {
                query.Add(x => x.UserName.Contains(request.UserName));
                isSearch = true;
            }

            if (!string.IsNullOrEmpty(request.FirstName))
            {
                var userIds = ISystemBaseServ.iUserProfileServ.GetAll(x => x.Firstname.Contains(request.FirstName)).Select(x => x.UserId);
                query.Add(x => userIds.Contains(x.Id));
                isSearch = true;
            }

            if (!string.IsNullOrEmpty(request.LastName))
            {
                var userIds = ISystemBaseServ.iUserProfileServ.GetAll(x => x.Lastname.Contains(request.LastName)).Select(x => x.UserId);
                query.Add(x => userIds.Contains(x.Id));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = ISystemBaseServ.iNikUserServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;


            ViewBag.PageTitle = "مدیریت دسته بندی ها";

            ViewBag.Contents = ISystemBaseServ.iNikUserServ.GetPart(query, pager.StartIndex, pager.PageSize, x => x.Id, true).ToList();

            return View();
        }

        [HttpGet]
        public IActionResult Create(int Id)
        {
            ViewBag.PageTitle = "ایجاد دسته بندی";

            var request = new UserRequest();

            if (Id > 0)
            {
                var item = ISystemBaseServ.iNikUserServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.UserName = item.UserName;
                request.Email = item.Email;
                request.PhoneNumber = item.PhoneNumber;
            }

            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserRequest request)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (!ValidUserForm(request))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            DataModel.User item = new DataModel.User();
            if (request.Id > 0)
            {
                item = await userManager.FindByIdAsync(request.Id.ToString());
            }


            item.UserName = request.UserName;
            item.Email = request.Email;
            item.PhoneNumber = request.PhoneNumber;


            if (request.Id == 0)
            {
                item.EmailConfirmed = true;
                item.PhoneNumberConfirmed = true;
                await userManager.CreateAsync(item, request.Password);
            }
            else
            {
                await userManager.UpdateAsync(item);
            }


            return Redirect("/Panel/UserManage");

        }

        public async Task<IActionResult> Remove(int Id)
        {
            var user = await userManager.FindByIdAsync(Id.ToString());
            await userManager.DeleteAsync(user);
            return Redirect("/Panel/UserManage");
        }

        public bool ValidUserForm(UserRequest request)
        {
            bool result = true;
            if (string.IsNullOrEmpty(request.UserName))
            {
                AddError("نام کاربری باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.Email))
            {
                AddError("آدرس ایمیل باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                AddError("رمز عبور باید مقدار داشته باشد", "fa");
                result = false;
            }
            else if (request.Password.Length < 6)
            {
                AddError("رمز عبور باید بیشتر از 6 کاراکتر باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.PhoneNumber))
            {
                AddError("شماره موبایل باید مقدار داشته باشد", "fa");
                result = false;
            }



            return result;
        }


        public async Task<IActionResult> ImortUsers(int start, int size)
        {
            ViewBag.Messages = Messages;
            if (size > 0)
            {
                var readyData = ISystemBaseServ.GetSubmitUsers(start, size);

                foreach (var item in readyData)
                {
                    ISystemBaseServ.iUserProfileServ.Add(new UserProfile
                    {
                        Firstname = item.Name,
                        Address = item.Address,
                        Tel = item.Phone,
                        UserId = item.UserId
                    });

                }

                await ISystemBaseServ.iUserProfileServ.SaveChangesAsync();

                AddSuccess("با موفقیت انجام شد.", "fa");
                ViewBag.Messages = Messages;
            }


            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ShowUserRoles(UserRoleRequest request)
        {
            var theUser = await userManager.FindByIdAsync(request.UserId.ToString());

            var rolNames = await userManager.GetRolesAsync(theUser);

            var userRoles = ISystemBaseServ.iNikRoleServ.GetAll(x => rolNames.Contains(x.Name)).ToList();

            ViewBag.PageTitle = "مدیریت نقش ها " + theUser.UserName;

            var allRoles = roleManager.Roles.Where(x => true).Select(x => new { x.Id, x.Name }).ToList();

            ViewBag.User = theUser;
            ViewBag.Roles = new SelectList(allRoles, "Id", "Name", request?.RoleId);
            ViewBag.Contents = userRoles.ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(UserRoleRequest request)
        {
            var theUser = await userManager.FindByIdAsync(request.UserId.ToString());
            var theRole = await ISystemBaseServ.iNikRoleServ.FindAsync(x => x.Id == request.RoleId);
            await userManager.AddToRoleAsync(theUser, theRole.Name);
            return Redirect("/Panel/UserManage/ShowUserRoles?UserId=" + request.UserId);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveRole(UserRoleRequest request)
        {
            var theUser = await userManager.FindByIdAsync(request.UserId.ToString());
            var theRole = await ISystemBaseServ.iNikRoleServ.FindAsync(x => x.Id == request.RoleId);
            await userManager.RemoveFromRoleAsync(theUser, theRole.Name);
            return Redirect("/Panel/UserManage/ShowUserRoles?UserId=" + request.UserId);
        }


    }
}
