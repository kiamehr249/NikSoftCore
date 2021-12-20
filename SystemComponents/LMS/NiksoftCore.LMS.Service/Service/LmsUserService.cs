using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.LMS.Service
{
    public interface ILmsUserService : IDataService<LmsUser>
    {
    }

    public class LmsUserService : DataService<LmsUser>, ILmsUserService
    {
        public LmsUserService(ILmsUnitOfWork uow) : base(uow)
        {
        }

        public override IList<LmsUser> GetPartOptional(List<Expression<Func<LmsUser, bool>>> predicate, int startIndex, int pageSize)
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