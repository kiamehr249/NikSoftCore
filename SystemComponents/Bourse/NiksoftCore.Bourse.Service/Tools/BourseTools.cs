using Newtonsoft.Json;
using System.Collections.Generic;

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

        public static List<ObjectItem> GetObjectList(this string strObj)
        {
            return JsonConvert.DeserializeObject<List<ObjectItem>>(strObj);
        }
    }
}
