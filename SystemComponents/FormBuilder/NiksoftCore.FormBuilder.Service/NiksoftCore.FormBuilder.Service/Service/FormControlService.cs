using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.FormBuilder.Service
{
    public interface IFormControlService : IDataService<FormControl>
    {
    }

    public class FormControlService : DataService<FormControl>, IFormControlService
    {
        public FormControlService(IFormUnitOfWork uow) : base(uow)
        {
        }

        public override IList<FormControl> GetPartOptional(List<Expression<Func<FormControl, bool>>> predicate, int startIndex, int pageSize)
        {
            var query = TEntity.Where(predicate[0]);
            for (int i = 1; i < predicate.Count; i++)
            {
                query = query.Where(predicate[i]);
            }
            return query.OrderByDescending(i => i.OrderId).ThenBy(x => x.Id).Skip(startIndex).Take(pageSize).ToList();
        }
    }
}