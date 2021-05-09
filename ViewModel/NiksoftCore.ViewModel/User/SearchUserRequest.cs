using System.ComponentModel.DataAnnotations;

namespace NiksoftCore.ViewModel
{
    public class SearchUserRequest
    {
        [Required(ErrorMessage = "عنوان نمی تواند خالی باشد")]
        public string Title { get; set; }
        public int part { get; set; }
    }
}
