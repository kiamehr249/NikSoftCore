namespace NiksoftCore.Bourse.Service
{
    public static class BourseTools
    {
        public static string GetContractStatus(this ContractStatus status)
        {
            switch (status)
            {
                case ContractStatus.Save:
                    return "ثبت شده";
                case ContractStatus.InProccess:
                    return "در جریان";
                case ContractStatus.Accept:
                    return "تایید شده";
                case ContractStatus.Ignore:
                    return "مختومه";
                default:
                    return "نامشخص";
            }
        }

        public static string GetMediaStatus(this MediaStatus status)
        {
            switch (status)
            {
                case MediaStatus.Save:
                    return "ثبت شده";
                case MediaStatus.InProccess:
                    return "در جریان";
                case MediaStatus.Accept:
                    return "تایید شده";
                case MediaStatus.Ignore:
                    return "مختومه";
                default:
                    return "نامشخص";
            }
        }

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
                case 3:
                    return "مبلغ شعبه";
                default:
                    return "نامشخص";
            }
        }

        public static string ReceiptStatusName(this ReceiptStatus type)
        {
            switch (type)
            {
                case ReceiptStatus.Save:
                    return "ثبت اولیه";
                case ReceiptStatus.Accept:
                    return "تاید سند";
                default:
                    return "نامشخص";
            }
        }

        public static string ContractTypeName(this ContractType type)
        {
            switch (type)
            {
                case ContractType.Marketer:
                    return "بازاریاب شعبه";
                case ContractType.Consultant:
                    return "مشاور بازاریابی";
                case ContractType.AdLeader:
                    return "مبلغ";
                case ContractType.Advertiser:
                    return "مبلغین";
                default:
                    return "نامشخص";
            }
        }

        public static string TicketStatusName(this TicketStatus type)
        {
            switch (type)
            {
                case TicketStatus.Registered:
                    return "در انتظار پاسخ";
                case TicketStatus.Answered:
                    return "پاسخ داده شده";
                case TicketStatus.Rejected:
                    return "عدم پاسخ";
                case TicketStatus.Seen:
                    return "درحال بررسی";
                default:
                    return "نامشخص";
            }
        }
    }
}
