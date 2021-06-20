using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.Bourse.Service
{
    public interface ITicketPriorityService : IDataService<TicketPriority>
    {
    }

    public class TicketPriorityService : DataService<TicketPriority>, ITicketPriorityService
    {
        public TicketPriorityService(IBourseUnitOfWork uow) : base(uow)
        {
        }

        public override IList<TicketPriority> GetPartOptional(List<Expression<Func<TicketPriority, bool>>> predicate, int startIndex, int pageSize)
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