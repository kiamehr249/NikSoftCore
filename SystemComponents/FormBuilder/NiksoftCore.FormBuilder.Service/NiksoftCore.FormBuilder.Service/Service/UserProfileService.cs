using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.FormBuilder.Service
{
    public interface IUserProfileService : IDataService<FormUserProfile>
    {
    }

    public class UserProfileService : DataService<FormUserProfile>, IUserProfileService
    {
        public UserProfileService(IFormUnitOfWork uow) : base(uow)
        {
        }

        public override IList<FormUserProfile> GetPartOptional(List<Expression<Func<FormUserProfile, bool>>> predicate, int startIndex, int pageSize)
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