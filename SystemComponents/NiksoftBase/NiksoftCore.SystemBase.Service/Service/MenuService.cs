using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.SystemBase.Service
{
    public interface IMenuService : IDataService<Menu>
    {
    }

    public class MenuService : DataService<Menu>, IMenuService
    {
        public MenuService(ISystemUnitOfWork uow) : base(uow)
        {
        }

        public override IList<Menu> GetPartOptional(List<Expression<Func<Menu, bool>>> predicate, int startIndex, int pageSize)
        {
            var query = TEntity.Where(predicate[0]);
            for (int i = 1; i < predicate.Count; i++)
            {
                query = query.Where(predicate[i]);
            }
            return query.OrderByDescending(i => i.OrderId).ThenBy(t => t.Id).Skip(startIndex).Take(pageSize).ToList();
        }
    }
}