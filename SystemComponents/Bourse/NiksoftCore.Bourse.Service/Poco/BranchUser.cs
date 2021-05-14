namespace NiksoftCore.Bourse.Service
{
    public class BranchUser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public BranchUserType UserType { get; set; }

        public virtual BourseUser User { get; set; }
        public virtual Branch Branch { get; set; }
    }
}
