﻿using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.SystemBase.Service
{
    public interface IPortalLanguageService : IDataService<PortalLanguage>
    {
    }

    public class PortalLanguageService : DataService<PortalLanguage>, IPortalLanguageService
    {
        public PortalLanguageService(ISystemUnitOfWork uow) : base(uow)
        {
        }

        public override IList<PortalLanguage> GetPartOptional(List<Expression<Func<PortalLanguage, bool>>> predicate, int startIndex, int pageSize)
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