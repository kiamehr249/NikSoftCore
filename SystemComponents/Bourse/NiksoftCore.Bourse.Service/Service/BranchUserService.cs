using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.Bourse.Service
{
    public interface IBranchUserService : IDataService<BranchUser>
    {
    }

    public class BranchUserService : DataService<BranchUser>, IBranchUserService
    {
        public BranchUserService(IBourseUnitOfWork uow) : base(uow)
        {
        }

        public override IList<BranchUser> GetPartOptional(List<Expression<Func<BranchUser, bool>>> predicate, int startIndex, int pageSize)
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