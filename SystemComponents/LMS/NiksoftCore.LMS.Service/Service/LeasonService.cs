using NiksoftCore.DataAccess;

namespace NiksoftCore.LMS.Service
{
    public interface ILeasonService : IDataService<Leason>
    {
    }

    public class LeasonService : DataService<Leason>, ILeasonService
    {
        public LeasonService(ILmsUnitOfWork uow) : base(uow)
        {
        }
    }
}