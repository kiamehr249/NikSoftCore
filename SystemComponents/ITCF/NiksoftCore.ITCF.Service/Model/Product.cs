using System.Collections.Generic;

namespace NiksoftCore.ITCF.Service
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Video { get; set; }
        public int CategoryId { get; set; }
        public int BusinessId { get; set; }

        public virtual BusinessCategory BusinessCategory { get; set; }
        public virtual Business Business { get; set; }

        public virtual ICollection<UserPurchase> Purchases { get; set; }
    }
}
