using NiksoftCore.DataAccess;

namespace NiksoftCore.LMS.Service
{
    public interface IUserCourseService : IDataService<UserCourse>
    {
    }

    public class UserCourseService : DataService<UserCourse>, IUserCourseService
    {
        public UserCourseService(ILmsUnitOfWork uow) : base(uow)
        {
        }
    }
}