using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.ITCF.Service
{
    public interface IProductFileService : IDataService<ProductFile>
    {
    }

    public class ProductFileService : DataService<ProductFile>, IProductFileService
    {
        public ProductFileService(IITCFUnitOfWork uow) : base(uow)
        {
        }

        public override IList<ProductFile> GetPartOptional(List<Expression<Func<ProductFile, bool>>> predicate, int startIndex, int pageSize)
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