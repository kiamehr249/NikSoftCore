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
        Rejected = 2,
        [Display(Name = "پیش پرداخت")]
        PrePayment = 3,
        [Display(Name = "تسویه کامل")]
        CompletePayemnt = 4,
        [Display(Name = "در حال ارسال")]
        Sending = 5,
        [Display(Name = "تحویل داده شد")]
        Delivered = 6
    }
}