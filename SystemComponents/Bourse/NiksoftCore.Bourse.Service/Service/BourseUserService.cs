using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.Bourse.Service
{
    public interface IBourseUserService : IDataService<BourseUser>
    {
    }

    public class BourseUserService : DataService<BourseUser>, IBourseUserService
    {
        public BourseUserService(IBourseUnitOfWork uow) : base(uow)
        {
        }

        public override IList<BourseUser> GetPartOptional(List<Expression<Func<BourseUser, bool>>> predicate, int startIndex, int pageSize)
        {
            var query = TEntity.Where(predicate[0]);
            for (int i = 1; i < predicate.Count; i++)
            {
                query = query.Where(predicate[i]);
            }
            return query.OrderBy(i => i.Id).ThenBy(t => t.Id).Skip(startIndex).Take(pageSize).ToList();
        }
    }
}