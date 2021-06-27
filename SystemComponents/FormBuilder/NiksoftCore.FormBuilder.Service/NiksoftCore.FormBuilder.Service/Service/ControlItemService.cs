using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.FormBuilder.Service
{
    public interface IControlItemService : IDataService<ControlItem>
    {
    }

    public class ControlItemService : DataService<ControlItem>, IControlItemService
    {
        public ControlItemService(IFormUnitOfWork uow) : base(uow)
        {
        }

        public override IList<ControlItem> GetPartOptional(List<Expression<Func<ControlItem, bool>>> predicate, int startIndex, int pageSize)
        {
            var query = TEntity.Where(predicate[0]);
            for (int i = 1; i < predicate.Count; i++)
            {
                query = query.Where(predicate[i]);
            }
            return query.OrderBy(i => i.OrderId).Skip(startIndex).Take(pageSize).ToList();
        }
    }
}