using Microsoft.AspNetCore.Identity;

namespace NiksoftCore.DataModel
{
    public class User : IdentityUser<int>
    {
        public AccountType AccountType { get; set; }
    }
}
