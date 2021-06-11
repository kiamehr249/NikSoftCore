using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.Bourse.Service
{
    public interface IBranchAdvertiserService : IDataService<BranchAdvertiser>
    {
    }

    public class BranchAdvertiserService : DataService<BranchAdvertiser>, IBranchAdvertiserService
    {
        public BranchAdvertiserService(IBourseUnitOfWork uow) : base(uow)
        {
        }

        public override IList<BranchAdvertiser> GetPartOptional(List<Expression<Func<BranchAdvertiser, bool>>> predicate, int startIndex, int pageSize)
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