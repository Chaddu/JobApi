using Domain.Common;

namespace Application.Interfaces.Repositories;
public interface IGenericRepository<T> where T : BaseEntity<int>
{
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    void SoftDelete(T entity, int? deletedById = null);
    Task<bool> ExistsAsync(int id);
}
