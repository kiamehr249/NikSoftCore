using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.Bourse.Service
{
    public interface IPricingPackageService : IDataService<PricingPackage>
    {
    }

    public class PricingPackageService : DataService<PricingPackage>, IPricingPackageService
    {
        public PricingPackageService(IBourseUnitOfWork uow) : base(uow)
        {
        }

        public override IList<PricingPackage> GetPartOptional(List<Expression<Func<PricingPackage, bool>>> predicate, int startIndex, int pageSize)
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