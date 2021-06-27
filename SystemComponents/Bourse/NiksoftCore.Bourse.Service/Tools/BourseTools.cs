namespace NiksoftCore.Bourse.Service
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
    }
}
