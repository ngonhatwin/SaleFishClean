using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
namespace Contract.Interfaces
{
    public interface IRepositoryBaseAsync<T, K> 
        where T : class
    {
        IQueryable<T> FindAll(bool trackChanges = false);
        //params là một từ khóa trong C#
        //cho phép bạn truyền một số lượng biến tham số không cố định cho một phương thức hoặc một mảng
        //Expression<Func<T, object>>[] là một mảng các biểu thức lambda,
        //mỗi biểu thức là một phương thức có một đối số kiểu T và trả về một giá trị kiểu object.
        //Trong trường hợp này, nó đại diện cho danh sách các thuộc tính mà bạn muốn bao gồm trong kết quả trả về
        IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false,
            params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetByIdAsync(K id);
        Task<IEnumerable<T>> GetAllAsync();
        Task CreateAsync(T entity);
        Task UpdateAsync(K id, T entity);
        Task DeleteAsync(T entity);
        Task SaveChangesAsync();
        Task BeginTransactionAsync();
        Task RollBackTransactionAsync();
        Task EndTransactionAsync();


    }
    public interface IRepositoryBaseAsync<TContext, T, K> : IRepositoryBaseAsync<T, K>
        where TContext : DbContext
        where T : class
    {

    }    
}
