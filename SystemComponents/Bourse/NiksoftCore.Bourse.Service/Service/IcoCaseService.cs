using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.Bourse.Service
{
    public interface IIcoCaseService : IDataService<IcoCase>
    {
    }

    public class IcoCaseService : DataService<IcoCase>, IIcoCaseService
    {
        public IcoCaseService(IBourseUnitOfWork uow) : base(uow)
        {
        }

        public override IList<IcoCase> GetPartOptional(List<Expression<Func<IcoCase, bool>>> predicate, int startIndex, int pageSize)
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