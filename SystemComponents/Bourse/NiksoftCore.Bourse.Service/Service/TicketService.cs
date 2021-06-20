using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.Bourse.Service
{
    public interface ITicketService : IDataService<Ticket>
    {
    }

    public class TicketService : DataService<Ticket>, ITicketService
    {
        public TicketService(IBourseUnitOfWork uow) : base(uow)
        {
        }

        public override IList<Ticket> GetPartOptional(List<Expression<Func<Ticket, bool>>> predicate, int startIndex, int pageSize)
        {
            var query = TEntity.Where(predicate[0]);
            for (int i = 1; i < predicate.Count; i++)
            {
                query = query.Where(predicate[i]);
            }
            return query.OrderByDescending(i => i.Id).Skip(startIndex).Take(pageSize).ToList();
        }
    }
}