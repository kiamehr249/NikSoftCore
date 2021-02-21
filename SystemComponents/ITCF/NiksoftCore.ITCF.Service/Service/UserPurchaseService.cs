using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.ITCF.Service
{
    public interface IUserPurchaseService : IDataService<UserPurchase>
    {
    }

    public class UserPurchaseService : DataService<UserPurchase>, IUserPurchaseService
    {
        public UserPurchaseService(IITCFUnitOfWork uow) : base(uow)
        {
        }

        public override IList<UserPurchase> GetPartOptional(List<Expression<Func<UserPurchase, bool>>> predicate, int startIndex, int pageSize)
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