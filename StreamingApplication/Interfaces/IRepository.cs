using System.Collections.Generic;
using System.Threading.Tasks;
using StreamingApplication.Data.Base;
using StreamingApplication.Helpers.Parameters;


namespace StreamingApplication.Interfaces;

public interface IRepository<T> where T : BaseEntity {
    Task<T> CreateAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task<List<T>> GetAllAsync(RequestParameters parameters);
    Task<T?> GetByIdAsync(int id);
    Task<T?> UpdateAsync(int id, T entity);
}