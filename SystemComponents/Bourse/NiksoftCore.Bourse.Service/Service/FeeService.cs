using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.Bourse.Service
{
    public interface IFeeService : IDataService<Fee>
    {
    }

    public class FeeService : DataService<Fee>, IFeeService
    {
        public FeeService(IBourseUnitOfWork uow) : base(uow)
        {
        }

        public override IList<Fee> GetPartOptional(List<Expression<Func<Fee, bool>>> predicate, int startIndex, int pageSize)
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