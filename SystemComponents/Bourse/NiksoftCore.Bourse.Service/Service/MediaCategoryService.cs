using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.Bourse.Service
{
    public interface IMediaCategoryService : IDataService<MediaCategory>
    {
    }

    public class MediaCategoryService : DataService<MediaCategory>, IMediaCategoryService
    {
        public MediaCategoryService(IBourseUnitOfWork uow) : base(uow)
        {
        }

        public override IList<MediaCategory> GetPartOptional(List<Expression<Func<MediaCategory, bool>>> predicate, int startIndex, int pageSize)
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