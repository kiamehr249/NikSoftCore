namespace NiksoftCore.ViewModel
{
    public class ChangePassRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPass { get; set; }
    }
}
