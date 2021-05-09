using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System.Linq;

namespace NiksoftCore.SystemBase.Controllers.General.User
{
    public class SearchUser : NikController
    {
        private readonly UserManager<DataModel.User> userManager;
        private readonly SignInManager<DataModel.User> signInManager;

        public SearchUser(UserManager<DataModel.User> userManager, SignInManager<DataModel.User> signInManager, IConfiguration Configuration) : base(Configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }


        [HttpGet]
        public IActionResult Index(SearchUserRequest request)
        {
            if (ModelState.IsValid)
            {
                //return View(request);
            }

            var query = ISystemBaseServ.iUserProfileServ.ExpressionMaker();
            query.Add(x => x.Status == ProfileStatus.Default);

            if (!string.IsNullOrEmpty(request.Title) && request.Title.Length > 2)
            {
                query.Add(x => x.CompanyName.Contains(request.Title));
            }
            else
            {
                query.Add(x => false);
            }

            var total = ISystemBaseServ.iUserProfileServ.Count(query);
            var pager = new Pagination(total, 10, 1);
            ViewBag.Pager = pager;

            ViewBag.PageTitle = "جستجوی کسب و کار";

            ViewBag.Contents = ISystemBaseServ.iUserProfileServ.GetPart(query, pager.StartIndex, pager.PageSize, x => x.Id, true).ToList();

            return View(request);
        }

        [HttpGet]
        public IActionResult RegisterUserInfo(int Id)
        {

            ViewBag.PageTitle = "ثبت اطلاعات کاریری";

            var request = new UserProfileRequest();

            if (Id > 0)
            {
                var item = ISystemBaseServ.iUserProfileServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Firstname = item.Firstname;
                request.Lastname = item.Lastname;
                request.Mobile = item.Mobile;
                request.Status = item.Status;
            }


            return View(request);
        }

        [HttpPost]
        public IActionResult RegisterUserInfo(UserProfileRequest request)
        {

            ViewBag.PageTitle = "ثبت اطلاعات کاریری";

            if (string.IsNullOrEmpty(request.Lastname))
            {
                AddError("نام خانوادگی باید مقدار داشته باشد", "fa");
            }

            if (string.IsNullOrEmpty(request.Firstname))
            {
                AddError("نام باید مقدار داشته باشد", "fa");
            }

            if (string.IsNullOrEmpty(request.Mobile) || request.Mobile.Length < 10)
            {
                AddError("شماره موبایل را صحیح وارد", "fa");
            }

            ViewBag.Messages = Messages;

            if (Messages.Count(x => x.Type == ViewModel.MessageType.Error) > 0)
            {
                return View(request);
            }

            var item = ISystemBaseServ.iUserProfileServ.Find(x => x.Id == request.Id);
            item.Firstname = request.Firstname;
            item.Lastname = request.Lastname;
            item.Mobile = request.Mobile;
            item.Status = ProfileStatus.SendData;

            ISystemBaseServ.iUserProfileServ.SaveChanges();

            return Redirect("/");
        }

    }
}
