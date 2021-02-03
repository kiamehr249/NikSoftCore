namespace NiksoftCore.ViewModel
{
    public class UserGridRequest : BaseRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RoleId { get; set; }
    }
}
