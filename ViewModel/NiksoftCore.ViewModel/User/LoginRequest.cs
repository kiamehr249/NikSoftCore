using System.ComponentModel.DataAnnotations;

namespace NiksoftCore.ViewModel.User
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "نام کاربری نمی تواند خالی باشد")]
        public string Username { get; set; }

        [Required(ErrorMessage = "رمز عبور نمی تواند خالی باشد.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }
}
