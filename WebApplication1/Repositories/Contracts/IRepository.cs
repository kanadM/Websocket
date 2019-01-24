using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Repositories.Contracts
{
    public interface IRepository<T> where T : class
    {
        void Initialize();
        List<string> ErrorMessages { get; set; }
        bool IsSuccessStatus { get; set; }
        IQueryable<T> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        void AddAsync(T entity);
        void DeleteAsync(T entity);
        void DeleteAsync(int id);
        void UpdateAsync(T entity);
    }
}
 