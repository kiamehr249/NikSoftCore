namespace NiksoftCore.FormBuilder.Service
{
    public static class BourseTools
    {

        public static string GetProfileTypeName(this int type)
        {
            switch (type)
            {
                case 0:
                    return "معمول";
                case 1:
                    return "بازاریاب شعبه";
                case 2:
                    return "کارمند شرکت";
                default:
                    return "نامشخص";
            }
        }

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
    }
}
