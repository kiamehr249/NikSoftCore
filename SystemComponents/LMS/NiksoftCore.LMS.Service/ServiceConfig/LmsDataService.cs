using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.LMS.Service
{
	public interface ILmsDataService
	{
		ILmsUserService iLmsUserServ { get; set; }
		ICalendarService iCalendarServ { get; set; }
		ICalendarDayService iCalendarDayServ { get; set; }
		ITermService iTermServ { get; set; }
		ICourseService iCourseServ { get; set; }
		ILeasonService iLeasonServ { get; set; }
		ILeasonFileService iLeasonFileServ { get; set; }
		IUserCourseService iUserCourseServ { get; set; }
	}

	public class LmsDataService : ILmsDataService
	{
		public IConfiguration config;
		public ILmsUserService iLmsUserServ { get; set; }
		public ICalendarService iCalendarServ { get; set; }
		public ICalendarDayService iCalendarDayServ { get; set; }
		public ITermService iTermServ { get; set; }
		public ICourseService iCourseServ { get; set; }
		public ILeasonService iLeasonServ { get; set; }
		public ILeasonFileService iLeasonFileServ { get; set; }
		public IUserCourseService iUserCourseServ { get; set; }


		public LmsDataService(IConfiguration config)
		{
			ILmsUnitOfWork uow = new LmsDbContext(config.GetConnectionString("SystemBase"));
			iLmsUserServ = new LmsUserService(uow);
			iCalendarServ = new CalendarService(uow);
			iCalendarDayServ = new CalendarDayService(uow);
			iTermServ = new TermService(uow);
			iCourseServ = new CourseService(uow);
			iLeasonServ = new LeasonService(uow);
			iLeasonFileServ = new LeasonFileService(uow);
			iUserCourseServ = new UserCourseService(uow);
		}



	}
}
