using NiksoftCore.DataAccess;

namespace NiksoftCore.LMS.Service
{
    public interface ICourseService : IDataService<Course>
    {
    }

    public class CourseService : DataService<Course>, ICourseService
    {
        public CourseService(ILmsUnitOfWork uow) : base(uow)
        {
        }
    }
}