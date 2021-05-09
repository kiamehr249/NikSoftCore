using System.ComponentModel.DataAnnotations;

namespace NiksoftCore.ViewModel.User
{
    public class UserRegisterRequest
    {
        [Required(ErrorMessage = "نام نمی تواند خالی باشد")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "نام خانوادگی نمی تواند خالی باشد")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "آدرس ایمیل نمی تواند خالی باشد")]
        public string Email { get; set; }

        [Required(ErrorMessage = "رمز عبور نمی تواند خالی باشد")]
        public string Password { get; set; }

        [Required(ErrorMessage = " تکرار رمز عبور نمی تواند خالی باشد")]
        [Compare("Password", ErrorMessage = "رمز عبور و تکرار آن یکسان نیست")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = " شماره موبایل نمی تواند خالی باشد")]
        public string PhoneNumber { get; set; }
    }
}
