namespace NiksoftCore.SystemBase.Service
{
    public class NikUserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public virtual NikUser NikUser { get; set; }
        public virtual NikRole NikRole { get; set; }

    }
}
