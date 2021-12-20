using NiksoftCore.DataAccess;

namespace NiksoftCore.LMS.Service
{
    public interface ICalendarService : IDataService<Calendar>
    {
    }

    public class CalendarService : DataService<Calendar>, ICalendarService
    {
        public CalendarService(ILmsUnitOfWork uow) : base(uow)
        {
        }
    }
}