using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.Bourse.Service
{
    public interface IBranchAdLeaderService : IDataService<BranchAdLeader>
    {
    }

    public class BranchAdLeaderService : DataService<BranchAdLeader>, IBranchAdLeaderService
    {
        public BranchAdLeaderService(IBourseUnitOfWork uow) : base(uow)
        {
        }

        public override IList<BranchAdLeader> GetPartOptional(List<Expression<Func<BranchAdLeader, bool>>> predicate, int startIndex, int pageSize)
        {
            var query = TEntity.Where(predicate[0]);
            for (int i = 1; i < predicate.Count; i++)
            {
                query = query.Where(predicate[i]);
            }
            return query.OrderByDescending(i => i.Id).ThenBy(t => t.Id).Skip(startIndex).Take(pageSize).ToList();
        }
    }
}