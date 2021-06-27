using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.FormBuilder.Service
{
    public interface IFormDataService : IDataService<FormData>
    {
    }

    public class FormDataService : DataService<FormData>, IFormDataService
    {
        public FormDataService(IFormUnitOfWork uow) : base(uow)
        {
        }

        public override IList<FormData> GetPartOptional(List<Expression<Func<FormData, bool>>> predicate, int startIndex, int pageSize)
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