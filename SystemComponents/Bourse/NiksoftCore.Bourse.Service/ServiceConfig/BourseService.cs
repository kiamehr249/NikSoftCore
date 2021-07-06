using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;

namespace NiksoftCore.Bourse.Service
{
    public interface IBourseService
    {
        BourseDbContext dbContext { get; }
        IBourseUserService iBourseUserServ { get; set; }
        IUserProfileService iUserProfileServ { get; set; }
        IPricingPackageService iPricingPackageServ { get; set; }
        IIcoCaseService iIcoCaseServ { get; set; }
    }

    public class BourseService : IBourseService
    {
        public BourseDbContext dbContext { get; }
        public IBourseUserService iBourseUserServ { get; set; }
        public IUserProfileService iUserProfileServ { get; set; }
        public IPricingPackageService iPricingPackageServ { get; set; }
        public IIcoCaseService iIcoCaseServ { get; set; }

        public BourseService(string connection)
        {
            dbContext = new BourseDbContext(connection);
            IBourseUnitOfWork uow = dbContext;
            iBourseUserServ = new BourseUserService(uow);
            iUserProfileServ = new UserProfileService(uow);
            iPricingPackageServ = new PricingPackageService(uow);
            iIcoCaseServ = new IcoCaseService(uow);
        }

    }
}
