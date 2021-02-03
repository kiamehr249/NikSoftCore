using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.SystemBase.Service
{
    public interface INikUserRoleService : IDataService<NikUserRole>
    {
    }

    public class NikUserRoleService : DataService<NikUserRole>, INikUserRoleService
    {
        public NikUserRoleService(ISystemUnitOfWork uow) : base(uow)
        {
        }

        public override IList<NikUserRole> GetPartOptional(List<Expression<Func<NikUserRole, bool>>> predicate, int startIndex, int pageSize)
        {
            var query = TEntity.Where(predicate[0]);
            for (int i = 1; i < predicate.Count; i++)
            {
                query = query.Where(predicate[i]);
            }
            return query.OrderBy(i => i.UserId).ThenBy(t => t.UserId).Skip(startIndex).Take(pageSize).ToList();
        }
    }
}