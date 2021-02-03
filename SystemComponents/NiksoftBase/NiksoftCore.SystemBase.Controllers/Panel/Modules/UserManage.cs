using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            if (!string.IsNullOrEmpty(request.lang))
                request.lang = request.lang.ToLower();
            else
                request.lang = defaultLang.ShortName.ToLower();

            var query = ISystemBaseServ.iNikUserServ.ExpressionMaker();
            query.Add(x => true);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.UserName))
            {
                query.Add(x => x.UserName.Contains(request.UserName));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = ISystemBaseServ.iNikUserServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;


            if (request.lang == "fa")
                ViewBag.PageTitle = "مدیریت دسته بندی ها";
            else
                ViewBag.PageTitle = "Business Category Management";

            ViewBag.Contents = ISystemBaseServ.iNikUserServ.GetPart(query, pager.StartIndex, pager.PageSize, x => x.Id, true).ToList();

            return View(GetViewName(request.lang, "Index"));
        }

        [HttpGet]
        public IActionResult Create([FromQuery] string lang, int Id)
        {
            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (lang == "fa")
                ViewBag.PageTitle = "ایجاد دسته بندی";
            else
                ViewBag.PageTitle = "Create Business Category";

            var request = new UserRequest();

            if (Id > 0)
            {
                var item = ISystemBaseServ.iNikUserServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.UserName = item.UserName;
                request.Email = item.Email;
                request.PhoneNumber = item.PhoneNumber;
            }

            return View(GetViewName(lang, "Create"), request);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] string lang, UserRequest request)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (!string.IsNullOrEmpty(lang))
                lang = lang.ToLower();
            else
                lang = defaultLang.ShortName.ToLower();

            if (!ValidUserForm(lang, request))
            {
                ViewBag.Messages = Messages;
                return View(GetViewName(lang, "Create"), request);
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

        public bool ValidUserForm(string lang, UserRequest request)
        {
            bool result = true;
            if (string.IsNullOrEmpty(request.UserName))
            {
                if (lang == "fa")
                    AddError("نام کاربری باید مقدار داشته باشد", "fa");
                else
                    AddError("User name can not be null", "en");
                result = false;
            }

            if (string.IsNullOrEmpty(request.Email))
            {
                if (lang == "fa")
                    AddError("آدرس ایمیل باید مقدار داشته باشد", "fa");
                else
                    AddError("Email can not be null", "en");
                result = false;
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                if (lang == "fa")
                    AddError("رمز عبور باید مقدار داشته باشد", "fa");
                else
                    AddError("ٍPassword can not be null", "en");
                result = false;
            }
            else if (request.Password.Length < 6)
            {
                if (lang == "fa")
                    AddError("رمز عبور باید بیشتر از 6 کاراکتر باشد", "fa");
                else
                    AddError("Password can not be null", "en");
                result = false;
            }

            if (string.IsNullOrEmpty(request.PhoneNumber))
            {
                if (lang == "fa")
                    AddError("شماره موبایل باید مقدار داشته باشد", "fa");
                else
                    AddError("Phone number can not be null", "en");

                result = false;
            }



            return result;
        }


        public async Task<IActionResult> ImortUsers(int start, int size)
        {
            ViewBag.Messages = Messages;
            if (size > 0)
            {
                int cc1 = 0;
                var imUsers = ISystemBaseServ.iBaseImportServ.GetPart(x => x.NCode != null && x.Email != null, start, size);
                foreach (var item in imUsers)
                {
                    cc1++;
                    await userManager.CreateAsync(new DataModel.User
                    {
                        UserName = item.Email,
                        Email = item.Email,
                        EmailConfirmed = true
                    }, item.NCode);
                }

                AddSuccess("تعداد " + cc1 + "با موفقیت انجام شد.", "fa");
                ViewBag.Messages = Messages;
            }


            return View();
        }


    }
}
