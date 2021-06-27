using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.FormBuilder.Service
{
    public interface IFormService : IDataService<Form>
    {
    }

    public class FormService : DataService<Form>, IFormService
    {
        public FormService(IFormUnitOfWork uow) : base(uow)
        {
        }

        public override IList<Form> GetPartOptional(List<Expression<Func<Form, bool>>> predicate, int startIndex, int pageSize)
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