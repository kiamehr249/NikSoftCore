using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.SystemBase.Service
{
    public interface IContentCategoryService : IDataService<ContentCategory>
    {
    }

    public class ContentCategoryService : DataService<ContentCategory>, IContentCategoryService
    {
        public ContentCategoryService(ISystemUnitOfWork uow) : base(uow)
        {
        }

        public override IList<ContentCategory> GetPartOptional(List<Expression<Func<ContentCategory, bool>>> predicate, int startIndex, int pageSize)
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