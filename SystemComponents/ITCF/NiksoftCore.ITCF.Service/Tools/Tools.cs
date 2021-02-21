using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.ITCF.Service
{
    public static class Tools
    {
        public static string GetStatusName(BusinessStatus status, string lang)
        {
            switch (status)
            {
                case BusinessStatus.RegisterRequest:
                    if (lang == "en")
                    {
                        return "Waiting for confirme";
                    }
                    else
                    {
                        return "در انتظار تایید";
                    }
                    
                case BusinessStatus.RegisterConfirm:
                    if (lang == "en")
                    {
                        return "Confirmed";
                    }
                    else
                    {
                        return "تایید ثبت";
                    }
                case BusinessStatus.EditRequest:
                    if (lang == "en")
                    {
                        return "Waiting for edit request Confirme";
                    }
                    else
                    {
                        return "در انتظار تایید درخواست ویرایش";
                    }
                case BusinessStatus.EditConfirm:
                    if (lang == "en")
                    {
                        return "Edit request Confirmed";
                    }
                    else
                    {
                        return "تایید ویرایش";
                    }
                case BusinessStatus.ShowRequest:
                    if (lang == "en")
                    {
                        return "Request to show";
                    }
                    else
                    {
                        return "درخواست نمایش سایت";
                    }
                case BusinessStatus.ConfirmShow:
                    if (lang == "en")
                    {
                        return "Show in site";
                    }
                    else
                    {
                        return "نمایش در سایت";
                    }
                case BusinessStatus.Disabled:
                    if (lang == "en")
                    {
                        return "Disabled";
                    }
                    else
                    {
                        return "مردود شده";
                    }
                default:
                    if (lang == "en")
                    {
                        return "Unknown";
                    }
                    else
                    {
                        return "نامشخص";
                    }
            }
        }

        public static string GetUnitKey(int unit)
        {
            switch (unit)
            {
                case 1:
                    return "unit1";
                case 2:
                    return "unit2";
                case 3:
                    return "unit3";
                case 4:
                    return "unit4";
                case 5:
                    return "unit5";
                default:
                    return "";
            }
        }

        public static int GetUnitNum(string key)
        {
            switch (key)
            {
                case "unit1":
                    return 1;
                case "unit2":
                    return 2;
                case "unit3":
                    return 3;
                case "unit4":
                    return 4;
                case "unit5":
                    return 5;
                default:
                    return 0;
            }
        }

        public static string GetUnitTitle(int unit)
        {
            switch (unit)
            {
                case 1:
                    return "تصویر و متن جایگاه اول";
                case 2:
                    return "تصاویر و متن های جایگاه دوم";
                case 3:
                    return "محتوای جایگاه سوم";
                case 4:
                    return "unit4";
                case 5:
                    return "unit5";
                default:
                    return "";
            }
        }

        public static bool AcceptBtn(BusinessStatus status)
        {
            if (status == BusinessStatus.RegisterRequest)
            {
                return true;
            }
            if (status == BusinessStatus.RegisterConfirm)
            {
                return false;
            }
            if (status == BusinessStatus.EditRequest)
            {
                return true;
            }
            if (status == BusinessStatus.EditConfirm)
            {
                return false;
            }
            if (status == BusinessStatus.ShowRequest)
            {
                return true;
            }
            if (status == BusinessStatus.ConfirmShow)
            {
                return false;
            }
            return false;
        }

        public static string GetPurchaseStatusName(PurchaseStatus status)
        {
            switch (status)
            {
                case PurchaseStatus.Requested:
                    return "درخواست خرید";
                case PurchaseStatus.Confirmed:
                    return "تایید خرید";
                case PurchaseStatus.Rejected:
                    return "رد درخواست";
                default:
                    return "نا مشخص";
            }
        }
    }
}
