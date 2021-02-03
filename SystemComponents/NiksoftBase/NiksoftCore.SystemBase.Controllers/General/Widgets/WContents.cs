using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.SystemBase.Service;
using NiksoftCore.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.SystemBase.Controllers.General.Widgets
{
    public class WContents : ViewComponent
    {
        public IConfiguration Config { get; }
        public ISystemBaseService iSystemBaseServ { get; set; }

        public List<NikMessage> Messages;
        public PortalLanguage defaultLang;

        public WContents(IConfiguration Configuration)
        {
            Config = Configuration;
            Messages = new List<NikMessage>();
            iSystemBaseServ = new SystemBaseService(Config.GetConnectionString("SystemBase"));
            defaultLang = iSystemBaseServ.iPortalLanguageServ.Find(x => x.IsDefault);
        }

        public async Task<IViewComponentResult> InvokeAsync(string key, int size = 8)
        {
            var theCategory = iSystemBaseServ.iContentCategoryServ.Find(x => x.KeyValue == key);
            List<string> KeyValues;
            IList<GeneralContent> contents;
            IList<ContentCategory> categories;
            if (theCategory != null && theCategory.Childs.Count > 0)
            {
                categories = iSystemBaseServ.iContentCategoryServ.GetAll(x => x.Parent.KeyValue == key);
                KeyValues = categories.Select(x => x.KeyValue).ToList();
                contents = iSystemBaseServ.iGeneralContentServ.GetAll(x => KeyValues.Contains(x.ContentCategory.KeyValue));
                ViewBag.Categories = categories;
            }
            else
            {
                contents = iSystemBaseServ.iGeneralContentServ.GetPart(x => x.ContentCategory.KeyValue == key, 0, size, x => x.Id , true);
            }
            ViewBag.Contents = contents;
            switch (key)
            {
                case "m_bigslider":
                    return View("FaMSlider");
                case "m_part2":
                    return View("FaHomePart2");
                case "m_part3":
                    return View("FaHomePart3");
                case "m_part4":
                    return View("FaHomePart4");
                case "m_part5":
                    return View("FaHomePart5");
                case "b_bigslider":
                    return View("FaBSlider");
                case "b_part2":
                    return View("FaHomePart2");
                case "b_part3":
                    return View("FaHomePart3");
                case "b_part4":
                    return View("FaHomePart4");
                default:
                    return View();
            }
        }

    }
}
