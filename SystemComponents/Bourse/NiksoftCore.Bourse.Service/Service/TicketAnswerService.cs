using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.Bourse.Service
{
    public interface ITicketAnswerService : IDataService<TicketAnswer>
    {
    }

    public class TicketAnswerService : DataService<TicketAnswer>, ITicketAnswerService
    {
        public TicketAnswerService(IBourseUnitOfWork uow) : base(uow)
        {
        }

        public override IList<TicketAnswer> GetPartOptional(List<Expression<Func<TicketAnswer, bool>>> predicate, int startIndex, int pageSize)
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