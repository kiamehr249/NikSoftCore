using NiksoftCore.DataAccess;

namespace NiksoftCore.LMS.Service
{
    public interface ITermService : IDataService<Term>
    {
    }

    public class TermService : DataService<Term>, ITermService
    {
        public TermService(ILmsUnitOfWork uow) : base(uow)
        {
        }
    }
}