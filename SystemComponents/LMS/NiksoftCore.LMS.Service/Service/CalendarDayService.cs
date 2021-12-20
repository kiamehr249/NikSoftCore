using NiksoftCore.DataAccess;

namespace NiksoftCore.LMS.Service
{
    public interface ICalendarDayService : IDataService<CalendarDay>
    {
    }

    public class CalendarDayService : DataService<CalendarDay>, ICalendarDayService
    {
        public CalendarDayService(ILmsUnitOfWork uow) : base(uow)
        {
        }
    }
}