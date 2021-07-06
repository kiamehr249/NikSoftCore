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

        public WContents(IConfiguration Configuration)
        {
            Config = Configuration;
            Messages = new List<NikMessage>();
            iSystemBaseServ = new SystemBaseService(Config.GetConnectionString("SystemBase"));
        }

        public async Task<IViewComponentResult> InvokeAsync(string key, int size = 8, string viewName = "Default", int skip = 0)
        {
            var theCategory = await iSystemBaseServ.iContentCategoryServ.FindAsync(x => x.KeyValue == key);
            List<string> KeyValues;
            IList<GeneralContent> contents;
            IList<ContentCategory> categories;
            if (theCategory != null && theCategory.Childs.Count > 0)
            {
                categories = iSystemBaseServ.iContentCategoryServ.GetAll(x => x.Parent.KeyValue == key);
                KeyValues = categories.Select(x => x.KeyValue).ToList();
                contents = iSystemBaseServ.iGeneralContentServ.GetPart(x => KeyValues.Contains(x.ContentCategory.KeyValue), skip, size);
                ViewBag.Categories = categories;
            }
            else
            {
                contents = iSystemBaseServ.iGeneralContentServ.GetPart(x => x.ContentCategory.KeyValue == key, skip, size, x => x.Id , true);
            }
            ViewBag.Contents = contents;
            return View(viewName);
        }

    }
}
