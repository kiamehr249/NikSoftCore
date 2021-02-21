using NiksoftCore.DataModel;
using System;
using System.Collections.Generic;

namespace NiksoftCore.ITCF.Service
{
    public class UserModel
    {
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string SecurityStamp { get; set; }
        public string PasswordHash { get; set; }
        public bool EmailConfirmed { get; set; }
        public string NormalizedEmail { get; set; }
        public string Email { get; set; }
        public string NormalizedUserName { get; set; }
        public string UserName { get; set; }
        public int Id { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public AccountType AccountType { get; set; }

        public virtual ICollection<UserPurchase> UserPurchases { get; set; }
        public virtual ICollection<UserProfile> UserProfiles { get; set; }
    }
}
