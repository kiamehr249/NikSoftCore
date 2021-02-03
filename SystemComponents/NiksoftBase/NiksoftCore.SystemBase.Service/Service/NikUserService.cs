using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.SystemBase.Service
{
    public interface INikUserService : IDataService<NikUser>
    {
    }

    public class NikUserService : DataService<NikUser>, INikUserService
    {
        public NikUserService(ISystemUnitOfWork uow) : base(uow)
        {
        }

        public override IList<NikUser> GetPartOptional(List<Expression<Func<NikUser, bool>>> predicate, int startIndex, int pageSize)
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