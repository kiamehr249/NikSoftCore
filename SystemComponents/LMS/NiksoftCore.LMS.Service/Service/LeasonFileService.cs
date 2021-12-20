using NiksoftCore.DataAccess;

namespace NiksoftCore.LMS.Service
{
    public interface ILeasonFileService : IDataService<LeasonFile>
    {
    }

    public class LeasonFileService : DataService<LeasonFile>, ILeasonFileService
    {
        public LeasonFileService(ILmsUnitOfWork uow) : base(uow)
        {
        }
    }
}