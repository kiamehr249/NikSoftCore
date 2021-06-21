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
    [Authorize]
    public class TicketManager : NikController
    {
        public IBourseService iBourseServ { get; set; }
        private readonly UserManager<DataModel.User> userManager;

        public TicketManager(IConfiguration Configuration, UserManager<DataModel.User> userManager) : base(Configuration)
        {
            this.iBourseServ = new BourseService(Configuration.GetConnectionString("SystemBase"));
            this.userManager = userManager;
        }

        #region Category
        [Authorize(Roles = "NikAdmin,Admin")]
        public IActionResult Index(TicketCategorySearch request)
        {
            ViewBag.PageTitle = "مدیریت دسته بندی تیکت";

            var query = iBourseServ.iTicketCategoryServ.ExpressionMaker();
            query.Add(x => true);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iTicketCategoryServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iBourseServ.iTicketCategoryServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View();
        }

        [Authorize(Roles = "NikAdmin,Admin")]
        [HttpGet]
        public IActionResult CreateCategory(int Id)
        {
            ViewBag.PageTitle = "ایجاد دسته جدید";

            var request = new TicketCategoryRequest();
            if (Id > 0)
            {
                var item = iBourseServ.iTicketCategoryServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.KeyValue = item.KeyValue;
                request.Description = item.Description;
                request.Enabled = item.Enabled;
                request.OrderId = item.OrderId;

            }
            return View(request);
        }

        [Authorize(Roles = "NikAdmin,Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(TicketCategoryRequest request)
        {
            ViewBag.PageTitle = "ایجاد دسته جدید";
            ViewBag.Messages = Messages;
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (!ValidCategoryForm(request))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            TicketCategory item;
            if (request.Id > 0)
            {
                item = iBourseServ.iTicketCategoryServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }
            else
            {
                item = new TicketCategory();
                item.CreateDate = DateTime.Now;
                item.CreatedBy = user.Id;
            }

            item.Title = request.Title;
            item.KeyValue = request.KeyValue;
            item.Description = request.Description;

            if (request.Id == 0)
            {
                var maxOrder = iBourseServ.iTicketCategoryServ.Count(x => true);
                item.OrderId = maxOrder + 1;
                item.Enabled = true;
                iBourseServ.iTicketCategoryServ.Add(item);
            }

            await iBourseServ.iTicketCategoryServ.SaveChangesAsync();

            return Redirect("/Panel/TicketManager");

        }

        [Authorize(Roles = "NikAdmin,Admin")]
        public async Task<IActionResult> RemoveCategory(int Id)
        {
            var item = await iBourseServ.iTicketCategoryServ.FindAsync(x => x.Id == Id);
            iBourseServ.iTicketCategoryServ.Remove(item);
            await iBourseServ.iTicketCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/TicketManager");
        }

        private bool ValidCategoryForm(TicketCategoryRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("عنوان باید مقدار داشته باشد", "fa");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                return false;
            }

            return true;

        }

        [Authorize(Roles = "NikAdmin,Admin")]
        public async Task<IActionResult> OrderUpCat(int Id)
        {
            var theItem = iBourseServ.iTicketCategoryServ.Find(x => x.Id == Id);

            if (theItem.OrderId == 1)
            {
                return Redirect("/Panel/TicketManager");
            }

            var upItem = iBourseServ.iTicketCategoryServ.Find(x => x.OrderId == (theItem.OrderId - 1));

            theItem.OrderId = theItem.OrderId - 1;
            upItem.OrderId = upItem.OrderId + 1;

            await iBourseServ.iTicketCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/TicketManager");
        }

        [Authorize(Roles = "NikAdmin,Admin")]
        public async Task<IActionResult> OrderDownCat(int Id)
        {
            var theItem = iBourseServ.iTicketCategoryServ.Find(x => x.Id == Id);
            var itemsCount = iBourseServ.iTicketCategoryServ.Count(x => true);
            if (theItem.OrderId == itemsCount)
            {
                return Redirect("/Panel/TicketManager");
            }

            var upItem = iBourseServ.iTicketCategoryServ.Find(x => x.OrderId == (theItem.OrderId + 1));

            theItem.OrderId = theItem.OrderId + 1;
            upItem.OrderId = upItem.OrderId - 1;

            await iBourseServ.iTicketCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/TicketManager");
        }
        #endregion

        //Priority
        #region Priority
        [Authorize(Roles = "NikAdmin,Admin")]
        public IActionResult Priorities(TicketPrioritySearch request)
        {
            ViewBag.PageTitle = "مدیریت اولویت های تیکت";

            var query = iBourseServ.iTicketPriorityServ.ExpressionMaker();
            query.Add(x => true);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = iBourseServ.iTicketPriorityServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = iBourseServ.iTicketPriorityServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View();
        }

        [Authorize(Roles = "NikAdmin,Admin")]
        [HttpGet]
        public IActionResult CreatePriority(int Id)
        {
            ViewBag.PageTitle = "ایجاد اولویت جدید";

            var request = new TicketPriorityRequest();
            if (Id > 0)
            {
                var item = iBourseServ.iTicketPriorityServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.HexCode = item.HexCode;
                request.Description = item.Description;
            }
            return View(request);
        }

        [Authorize(Roles = "NikAdmin,Admin")]
        [HttpPost]
        public async Task<IActionResult> CreatePriority(TicketPriorityRequest request)
        {
            ViewBag.PageTitle = "ایجاداولویت جدید";
            ViewBag.Messages = Messages;
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (!ValidPriorityForm(request))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            TicketPriority item;
            if (request.Id > 0)
            {
                item = iBourseServ.iTicketPriorityServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }
            else
            {
                item = new TicketPriority();
                item.CreateDate = DateTime.Now;
                item.CreatedBy = user.Id;
            }

            item.Title = request.Title;
            item.HexCode = request.HexCode;
            item.Description = request.Description;

            if (request.Id == 0)
            {
                var maxOrder = iBourseServ.iTicketPriorityServ.Count(x => true);
                item.OrderId = maxOrder + 1;
                item.Enabled = true;
                iBourseServ.iTicketPriorityServ.Add(item);
            }

            await iBourseServ.iTicketPriorityServ.SaveChangesAsync();

            return Redirect("/Panel/TicketManager/Priorities");

        }

        [Authorize(Roles = "NikAdmin,Admin")]
        public async Task<IActionResult> RemovePriority(int Id)
        {
            var item = await iBourseServ.iTicketPriorityServ.FindAsync(x => x.Id == Id);
            iBourseServ.iTicketPriorityServ.Remove(item);
            await iBourseServ.iTicketPriorityServ.SaveChangesAsync();
            return Redirect("/Panel/TicketManager/Priorities");
        }

        private bool ValidPriorityForm(TicketPriorityRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("عنوان باید مقدار داشته باشد", "fa");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                return false;
            }

            return true;

        }

        [Authorize(Roles = "NikAdmin,Admin")]
        public async Task<IActionResult> OrderUpPriority(int Id)
        {
            var theItem = iBourseServ.iTicketPriorityServ.Find(x => x.Id == Id);

            if (theItem.OrderId == 1)
            {
                return Redirect("/Panel/TicketManager/Priorities");
            }

            var upItem = iBourseServ.iTicketPriorityServ.Find(x => x.OrderId == (theItem.OrderId - 1));

            theItem.OrderId = theItem.OrderId - 1;
            upItem.OrderId = upItem.OrderId + 1;

            await iBourseServ.iTicketPriorityServ.SaveChangesAsync();
            return Redirect("/Panel/TicketManager/Priorities");
        }

        [Authorize(Roles = "NikAdmin,Admin")]
        public async Task<IActionResult> OrderDownPriority(int Id)
        {
            var theItem = iBourseServ.iTicketPriorityServ.Find(x => x.Id == Id);
            var itemsCount = iBourseServ.iTicketPriorityServ.Count(x => true);
            if (theItem.OrderId == itemsCount)
            {
                return Redirect("/Panel/TicketManager/Priorities");
            }

            var upItem = iBourseServ.iTicketPriorityServ.Find(x => x.OrderId == (theItem.OrderId + 1));

            theItem.OrderId = theItem.OrderId + 1;
            upItem.OrderId = upItem.OrderId - 1;

            await iBourseServ.iTicketPriorityServ.SaveChangesAsync();
            return Redirect("/Panel/TicketManager/Priorities");
        }
        #endregion

        //User Ticket 
        #region User Ticket
        public async Task<IActionResult> MyTickets(MyTicketSearch request)
        {
            ViewBag.PageTitle = "تیکت های من";
            var user = await userManager.GetUserAsync(HttpContext.User);
            var query = iBourseServ.iTicketServ.ExpressionMaker();
            query.Add(x => x.UserId == user.Id);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title) || x.FullText.Contains(request.Title));
                isSearch = true;
            }

            if (request.PriorityId != 0)
            {
                query.Add(x => x.PriorityId == request.PriorityId);
                isSearch = true;
            }

            if (request.CategoryId != 0)
            {
                query.Add(x => x.CategoryId == request.CategoryId);
                isSearch = true;
            }

            ViewBag.Search = isSearch;
            var total = iBourseServ.iTicketServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;
            ViewBag.Contents = iBourseServ.iTicketServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            ComboBinder(request.CategoryId, request.PriorityId);
            return View();
        }

        [HttpGet]
        public IActionResult CreateTicket(int Id)
        {
            ViewBag.PageTitle = "ایجاد تیکت";
            var request = new TicketRequest();
            if (Id > 0)
            {
                var item = iBourseServ.iTicketServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.FullText = item.FullText;
                request.CategoryId = item.CategoryId;
                request.PriorityId = item.PriorityId;

            }
            ComboBinder(request.CategoryId, request.PriorityId);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(TicketRequest request)
        {
            ViewBag.PageTitle = "ایجاد تیکت";
            ViewBag.Messages = Messages;
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (!ValidTicketForm(request))
            {
                ViewBag.Messages = Messages;
                ComboBinder(request.CategoryId, request.PriorityId);
                return View(request);
            }

            Ticket item;
            if (request.Id > 0)
            {
                item = iBourseServ.iTicketServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
            }
            else
            {
                item = new Ticket();
                item.CreateDate = DateTime.Now;
            }

            item.Title = request.Title;
            item.FullText = request.FullText;
            item.CategoryId = request.CategoryId;
            item.PriorityId = request.PriorityId;

            var theCat = await iBourseServ.iTicketCategoryServ.FindAsync(x => x.Id == request.CategoryId);

            if (!string.IsNullOrEmpty(theCat.KeyValue.ToLower()))
            {
                if (theCat.KeyValue.ToLower() == "marketer")
                {
                    var consUser = await iBourseServ.iBranchConsultantServ.FindAsync(x => x.UserId == user.Id);
                    if (consUser == null)
                    {
                        AddError("مجاز به استفاده از این دسته نیستید.");
                        ViewBag.Messages = Messages;
                        ComboBinder(request.CategoryId, request.PriorityId);
                        return View(request);
                    }
                    else
                    {
                        item.TargetId = consUser.MarketerId;
                    }
                }
            }

            if (request.Id == 0)
            {
                item.Status = TicketStatus.Registered;
                item.UserId = user.Id;
                iBourseServ.iTicketServ.Add(item);
            }

            await iBourseServ.iTicketServ.SaveChangesAsync();

            return Redirect("/Panel/TicketManager/MyTickets");

        }

        public async Task<IActionResult> RemoveTicket(int Id)
        {
            var item = await iBourseServ.iTicketServ.FindAsync(x => x.Id == Id);
            iBourseServ.iTicketServ.Remove(item);
            await iBourseServ.iTicketServ.SaveChangesAsync();
            return Redirect("/Panel/TicketManager/MyTickets");
        }

        private bool ValidTicketForm(TicketRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("عنوان باید مقدار داشته باشد");
            }

            if (string.IsNullOrEmpty(request.FullText))
            {
                AddError("متن باید مقدار داشته باشد");
            }
            else if (request.FullText.Length < 5)
            {
                AddError("متن نمی تواند کمتر از 5 کارکتر باشد");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                return false;
            }

            return true;

        }

        #endregion

        //Answer Ticket
        #region Answer Ticket
        [Authorize(Roles = "NikAdmin,Admin,Support")]
        public async Task<IActionResult> Tickets(MyTicketSearch request)
        {
            ViewBag.PageTitle = "تیکت ها";
            var user = await userManager.GetUserAsync(HttpContext.User);
            var query = iBourseServ.iTicketServ.ExpressionMaker();
            query.Add(x => x.TargetId == null);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title) || x.FullText.Contains(request.Title));
                isSearch = true;
            }

            if (request.PriorityId != 0)
            {
                query.Add(x => x.PriorityId == request.PriorityId);
                isSearch = true;
            }

            if (request.CategoryId != 0)
            {
                query.Add(x => x.CategoryId == request.CategoryId);
                isSearch = true;
            }

            if (request.Status != 0)
            {
                var status = (TicketStatus)(request.Status);
                query.Add(x => x.Status == status);
                isSearch = true;
            }

            ViewBag.Search = isSearch;
            var total = iBourseServ.iTicketServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;
            ViewBag.Contents = iBourseServ.iTicketServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            ComboBinder(request.CategoryId, request.PriorityId, request.Status);
            return View();
        }

        [Authorize(Roles = "NikAdmin,Admin,Support")]
        [HttpGet]
        public async Task<IActionResult> CreateAnswer(int TicketId, int Id)
        {
            ViewBag.PageTitle = "پاسخ به تیکت";
            var theTicket = await iBourseServ.iTicketServ.FindAsync(x => x.Id == TicketId);
            ViewBag.Ticket = theTicket;
            var request = new TicketAnswerRequest();
            if (Id > 0)
            {
                var item = iBourseServ.iTicketAnswerServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.FullText = item.FullText;

            }
            else
            {
                request.Title = theTicket.Title;
            }
            request.TicketId = theTicket.Id;
            return View(request);
        }

        [Authorize(Roles = "NikAdmin,Admin,Support")]
        [HttpPost]
        public async Task<IActionResult> CreateAnswer(TicketAnswerRequest request)
        {
            ViewBag.PageTitle = "پاسخ به تیکت";
            ViewBag.Messages = Messages;
            var theTicket = await iBourseServ.iTicketServ.FindAsync(x => x.Id == request.TicketId);
            ViewBag.Ticket = theTicket;
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (!ValidAnswerForm(request))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            TicketAnswer item;
            if (request.Id > 0)
            {
                item = iBourseServ.iTicketAnswerServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
            }
            else
            {
                item = new TicketAnswer();
                item.CreateDate = DateTime.Now;
            }

            item.Title = request.Title;
            item.FullText = request.FullText;


            if (request.Id == 0)
            {
                item.TicketId = request.TicketId;
                item.UserId = user.Id;
                iBourseServ.iTicketAnswerServ.Add(item);
                theTicket.Status = TicketStatus.Answered;
                await iBourseServ.iTicketServ.SaveChangesAsync();
            }

            await iBourseServ.iTicketAnswerServ.SaveChangesAsync();

            return Redirect("/Panel/TicketManager/Tickets");

        }

        [Authorize(Roles = "NikAdmin,Admin,Support")]
        public async Task<IActionResult> RemoveAnswer(int Id)
        {
            var item = await iBourseServ.iTicketAnswerServ.FindAsync(x => x.Id == Id);
            var theTicket = await iBourseServ.iTicketServ.FindAsync(x => x.Id == item.TicketId);

            iBourseServ.iTicketAnswerServ.Remove(item);
            await iBourseServ.iTicketAnswerServ.SaveChangesAsync();

            if (theTicket.TicketAnswers.Count == 0)
            {
                theTicket.Status = TicketStatus.Registered;
                await iBourseServ.iTicketServ.SaveChangesAsync();
            }
            
            return Redirect("/Panel/TicketManager/Tickets");
        }

        private bool ValidAnswerForm(TicketAnswerRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("عنوان باید مقدار داشته باشد");
            }

            if (string.IsNullOrEmpty(request.FullText))
            {
                AddError("متن باید مقدار داشته باشد");
            }
            else if (request.FullText.Length < 5)
            {
                AddError("متن نمی تواند کمتر از 5 کارکتر باشد");
            }

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                return false;
            }

            return true;

        }

        [Authorize(Roles = "NikAdmin,Admin,Support")]
        public async Task<IActionResult> RejectTicket(int TicketId)
        {
            var item = await iBourseServ.iTicketServ.FindAsync(x => x.Id == TicketId);
            item.Status = TicketStatus.Rejected;
            await iBourseServ.iTicketServ.SaveChangesAsync();

            return Redirect("/Panel/TicketManager/Tickets");
        }
        #endregion

        //Marketer Answer
        #region Marketer Answer
        [Authorize(Roles = "NikAdmin,Admin,Marketer")]
        public async Task<IActionResult> MarketerTickets(MyTicketSearch request)
        {
            ViewBag.PageTitle = "تیکت ها";
            var user = await userManager.GetUserAsync(HttpContext.User);
            var query = iBourseServ.iTicketServ.ExpressionMaker();
            query.Add(x => x.TargetId == user.Id);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title) || x.FullText.Contains(request.Title));
                isSearch = true;
            }

            if (request.PriorityId != 0)
            {
                query.Add(x => x.PriorityId == request.PriorityId);
                isSearch = true;
            }

            if (request.CategoryId != 0)
            {
                query.Add(x => x.CategoryId == request.CategoryId);
                isSearch = true;
            }

            if (request.Status != 0)
            {
                var status = (TicketStatus)(request.Status);
                query.Add(x => x.Status == status);
                isSearch = true;
            }

            ViewBag.Search = isSearch;
            var total = iBourseServ.iTicketServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;
            ViewBag.Contents = iBourseServ.iTicketServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            ComboBinder(request.CategoryId, request.PriorityId, request.Status);
            return View();
        }

        [Authorize(Roles = "NikAdmin,Admin,Marketer")]
        [HttpGet]
        public async Task<IActionResult> SpecCreateAnswer(int TicketId, int Id)
        {
            ViewBag.PageTitle = "پاسخ به تیکت";
            var theTicket = await iBourseServ.iTicketServ.FindAsync(x => x.Id == TicketId);
            ViewBag.Ticket = theTicket;
            var request = new TicketAnswerRequest();
            if (Id > 0)
            {
                var item = iBourseServ.iTicketAnswerServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.FullText = item.FullText;

            }
            else
            {
                request.Title = theTicket.Title;
            }
            request.TicketId = theTicket.Id;
            return View(request);
        }

        [Authorize(Roles = "NikAdmin,Admin,Marketer")]
        [HttpPost]
        public async Task<IActionResult> SpecCreateAnswer(TicketAnswerRequest request)
        {
            ViewBag.PageTitle = "پاسخ به تیکت";
            ViewBag.Messages = Messages;
            var theTicket = await iBourseServ.iTicketServ.FindAsync(x => x.Id == request.TicketId);
            ViewBag.Ticket = theTicket;
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (!ValidAnswerForm(request))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            TicketAnswer item;
            if (request.Id > 0)
            {
                item = iBourseServ.iTicketAnswerServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
            }
            else
            {
                item = new TicketAnswer();
                item.CreateDate = DateTime.Now;
            }

            item.Title = request.Title;
            item.FullText = request.FullText;


            if (request.Id == 0)
            {
                item.TicketId = request.TicketId;
                item.UserId = user.Id;
                iBourseServ.iTicketAnswerServ.Add(item);
                theTicket.Status = TicketStatus.Answered;
                await iBourseServ.iTicketServ.SaveChangesAsync();
            }

            await iBourseServ.iTicketAnswerServ.SaveChangesAsync();

            return Redirect("/Panel/TicketManager/MarketerTickets");

        }

        [Authorize(Roles = "NikAdmin,Admin,Marketer")]
        public async Task<IActionResult> SpecRemoveAnswer(int Id)
        {
            var item = await iBourseServ.iTicketAnswerServ.FindAsync(x => x.Id == Id);
            var theTicket = await iBourseServ.iTicketServ.FindAsync(x => x.Id == item.TicketId);

            iBourseServ.iTicketAnswerServ.Remove(item);
            await iBourseServ.iTicketAnswerServ.SaveChangesAsync();

            if (theTicket.TicketAnswers.Count == 0)
            {
                theTicket.Status = TicketStatus.Registered;
                await iBourseServ.iTicketServ.SaveChangesAsync();
            }

            return Redirect("/Panel/TicketManager/MarketerTickets");
        }

        [Authorize(Roles = "NikAdmin,Admin,Marketer")]
        public async Task<IActionResult> SpecRejectTicket(int TicketId)
        {
            var item = await iBourseServ.iTicketServ.FindAsync(x => x.Id == TicketId);
            item.Status = TicketStatus.Rejected;
            await iBourseServ.iTicketServ.SaveChangesAsync();

            return Redirect("/Panel/TicketManager/MarketerTickets");
        }
        #endregion

        private void ComboBinder(int catId, int prioId, int status = 0)
        {
            var cats = iBourseServ.iTicketCategoryServ.GetAll(x => x.Enabled, y => new { y.Id, y.Title }).ToList();
            ViewBag.Categories = new SelectList(cats, "Id", "Title", catId);

            var priorities = iBourseServ.iTicketPriorityServ.GetAll(x => x.Enabled, y => new { y.Id, y.Title }).ToList();
            ViewBag.Priorities = new SelectList(priorities, "Id", "Title", prioId);

            List<ListItemModel> statusList = new List<ListItemModel>();
            statusList.Add(new ListItemModel
            {
                Id = 1,
                Title = "در انتظار پاسخ"
            });
            statusList.Add(new ListItemModel
            {
                Id = 2,
                Title = "درحال بررسی"
            });
            statusList.Add(new ListItemModel
            {
                Id = 3,
                Title = "پاسخ داده شده"
            });
            statusList.Add(new ListItemModel
            {
                Id = 4,
                Title = "عدم پاسخ"
            });
            ViewBag.Statuses = new SelectList(statusList, "Id", "Title", status);
        }

    }
}
