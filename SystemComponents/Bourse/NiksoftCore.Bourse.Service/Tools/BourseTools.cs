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
    }
}
