using Arch.EntityFrameworkCore.UnitOfWork;
using Contract.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq.Expressions;
namespace Contract
{
    public class RepositoryBaseAsync<TContext, T, K> :  IRepositoryBaseAsync<TContext, T, K>
       where TContext : DbContext
       where T : class
    {
        private readonly IUnitOfWork<TContext> _unitOfWork;
        public RepositoryBaseAsync( IUnitOfWork<TContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _unitOfWork.DbContext.Set<T>().AsNoTracking().ToListAsync();
        }
        public async Task<T> GetByIdAsync(K id)
        {
            var entity = await _unitOfWork.DbContext.Set<T>().FindAsync(id);
            _unitOfWork.DbContext.Entry(entity).State = EntityState.Detached;
            return entity;
        }
        public Task CreateAsync(T entity)
        {
            _unitOfWork.DbContext.Set<T>().Add(entity);
            return Task.CompletedTask;
        }
        public Task UpdateAsync(K id, T entity)
        {
            _unitOfWork.DbContext.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }
        public Task DeleteAsync(T entity)
        {
            _unitOfWork.DbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }
        public IQueryable<T> FindAll(bool trackChanges = false) =>
            !trackChanges ? _unitOfWork.DbContext.Set<T>().AsNoTracking() : _unitOfWork.DbContext.Set<T>();

        public IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
        {
            var items = FindAll(trackChanges);
            items = includeProperties.Aggregate(items, (current, includeProperties) => current.Include(includeProperties));
            return items;
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false)
        {
            return !trackChanges ? _unitOfWork.DbContext.Set<T>().Where(expression).AsNoTracking() : _unitOfWork.DbContext.Set<T>().Where(expression);
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
        {
            var items = FindByCondition(expression, trackChanges);
            items = includeProperties.Aggregate(items, (current, includeProperties) => current.Include(includeProperties));
            return items;
        }
        public async Task SaveChangesAsync() => await (_unitOfWork as IUnitOfWork).SaveChangesAsync();
        public async Task BeginTransactionAsync() => await _unitOfWork.DbContext.Database.BeginTransactionAsync();
        public async Task RollBackTransactionAsync() => await _unitOfWork.DbContext.Database.RollbackTransactionAsync();
        public async Task EndTransactionAsync()
        {
            await SaveChangesAsync();
            await _unitOfWork.DbContext.Database.CommitTransactionAsync();
        }


    }
}
