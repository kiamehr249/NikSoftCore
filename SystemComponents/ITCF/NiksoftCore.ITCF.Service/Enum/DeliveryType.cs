using System.ComponentModel.DataAnnotations;

namespace NiksoftCore.ITCF.Service
{
    public enum DeliveryType
    {
        [Display(Name = "پیش فرض")]
        Default = 0,
        [Display(Name = "با برند تولیدی")]
        Original = 1,
        [Display(Name = "با برند خارجی")]
        Representative = 2,
        [Display(Name = "با بسته بندی خارجی")]
        Packing = 3
    }
}