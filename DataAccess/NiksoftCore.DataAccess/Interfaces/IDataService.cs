using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NiksoftCore.DataAccess
{
    public interface IDataService<T> : IDisposable where T : class
    {
        int Update(T entity);
        Task<int> UpdateAsync(T entity);
        void Add(T entity);

        void Remove(T entity);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        int Count(Expression<Func<T, bool>> predicate);

        int Count(List<Expression<Func<T, bool>>> predicates);

        Task<T> FindAsync(Expression<Func<T, bool>> predicate);

        T Find(Expression<Func<T, bool>> predicate);

        IList<T> GetPartOptional(List<Expression<Func<T, bool>>> predicate, int startIndex, int size);

        IList<T> GetPart(Expression<Func<T, bool>> predicate, int startIndex, int size);

        IList<T> GetPart(Expression<Func<T, bool>> predicate, int startIndex, int size, Expression<Func<T, int>> keySelect, bool desc);

        IList<T> GetAll(Expression<Func<T, bool>> predicate);

        IList<T> GetAll(List<Expression<Func<T, bool>>> predicate);

        IList<TResult> GetAll<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selectItem);

        IList<TResult> GetAll<TResult>(List<Expression<Func<T, bool>>> predicate, Expression<Func<T, TResult>> selectItem);

        List<Expression<Func<T, bool>>> ExpressionMaker();

        int ExecSqlCommand(string command);

        Task<int> ExecSqlCommandAsync(string command);

        int ExecSqlCommand(string command, object[] values);

        Task<int> ExecSqlCommandAsync(string command, object[] values);

        List<T> GetDataFromSp(string command, object[] values);


        Task<int> SaveChangesAsync();

        int SaveChanges();

    }
}
