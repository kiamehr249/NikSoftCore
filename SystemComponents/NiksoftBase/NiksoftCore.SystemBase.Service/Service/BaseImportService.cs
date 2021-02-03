using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.SystemBase.Service
{
    public interface IBaseImportService : IDataService<BaseImport>
    {
    }

    public class BaseImportService : DataService<BaseImport>, IBaseImportService
    {
        public BaseImportService(ISystemUnitOfWork uow) : base(uow)
        {
        }

        public override IList<BaseImport> GetPartOptional(List<Expression<Func<BaseImport, bool>>> predicate, int startIndex, int pageSize)
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