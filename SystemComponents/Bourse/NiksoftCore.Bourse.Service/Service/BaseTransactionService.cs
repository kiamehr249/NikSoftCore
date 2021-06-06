using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.Bourse.Service
{
    public interface IBaseTransactionService : IDataService<BaseTransaction>
    {
    }

    public class BaseTransactionService : DataService<BaseTransaction>, IBaseTransactionService
    {
        public BaseTransactionService(IBourseUnitOfWork uow) : base(uow)
        {
        }

        public override IList<BaseTransaction> GetPartOptional(List<Expression<Func<BaseTransaction, bool>>> predicate, int startIndex, int pageSize)
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