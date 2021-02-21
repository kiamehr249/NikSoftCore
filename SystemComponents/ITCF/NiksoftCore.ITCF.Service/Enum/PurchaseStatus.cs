using System.ComponentModel.DataAnnotations;

namespace NiksoftCore.ITCF.Service
{
    public enum PurchaseStatus
    {
        [Display(Name = "در خواست ثبت شده")]
        Requested = 0,
        [Display(Name = "تایید در خواست")]
        Confirmed = 1,
        [Display(Name = "رد درخواست")]
        Rejected = 2
    }
}