using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.Bourse.Service
{
    public interface IBranchAreaService : IDataService<BranchArea>
    {
    }

    public class BranchAreaService : DataService<BranchArea>, IBranchAreaService
    {
        public BranchAreaService(IBourseUnitOfWork uow) : base(uow)
        {
        }

        public override IList<BranchArea> GetPartOptional(List<Expression<Func<BranchArea, bool>>> predicate, int startIndex, int pageSize)
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