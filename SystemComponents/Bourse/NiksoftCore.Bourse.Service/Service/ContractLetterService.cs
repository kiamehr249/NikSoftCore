using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.Bourse.Service
{
    public interface IContractLetterService : IDataService<ContractLetter>
    {
    }

    public class ContractLetterService : DataService<ContractLetter>, IContractLetterService
    {
        public ContractLetterService(IBourseUnitOfWork uow) : base(uow)
        {
        }

        public override IList<ContractLetter> GetPartOptional(List<Expression<Func<ContractLetter, bool>>> predicate, int startIndex, int pageSize)
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