using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.Bourse.Service
{
    public interface ITicketCategoryService : IDataService<TicketCategory>
    {
    }

    public class TicketCategoryService : DataService<TicketCategory>, ITicketCategoryService
    {
        public TicketCategoryService(IBourseUnitOfWork uow) : base(uow)
        {
        }

        public override IList<TicketCategory> GetPartOptional(List<Expression<Func<TicketCategory, bool>>> predicate, int startIndex, int pageSize)
        {
            var query = TEntity.Where(predicate[0]);
            for (int i = 1; i < predicate.Count; i++)
            {
                query = query.Where(predicate[i]);
            }
            return query.OrderBy(i => i.OrderId).ThenBy(t => t.Id).Skip(startIndex).Take(pageSize).ToList();
        }
    }
}