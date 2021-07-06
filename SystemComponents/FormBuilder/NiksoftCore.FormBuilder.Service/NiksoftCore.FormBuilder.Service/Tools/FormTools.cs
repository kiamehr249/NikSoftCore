using Newtonsoft.Json;
using System.Collections.Generic;

namespace NiksoftCore.FormBuilder.Service
{
    public static class FormTools
    {

        public static string GetControlTypeName(this ControlType type)
        {
            switch (type)
            {
                case ControlType.TextBox:
                    return "Text Box";
                case ControlType.TextArea:
                    return "Text Area";
                case ControlType.Editor:
                    return "Text Editor";
                case ControlType.FileUpload:
                    return "File Upload";
                case ControlType.CheckBox:
                    return "Check Box";
                case ControlType.DropDown:
                    return "Drop Down List";
                case ControlType.RadioList:
                    return "Radio Button List";
                default:
                    return "Unkown";
            }
        }

        public static List<FormAnswer> GetAnswerFromJson(this string strObj)
        {
            return JsonConvert.DeserializeObject<List<FormAnswer>>(strObj);
        }
    }
}
