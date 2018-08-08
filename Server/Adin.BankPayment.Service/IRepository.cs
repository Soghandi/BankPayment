using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Adin.BankPayment.Service
{
    public interface IRepository<TEntity>
    {
        Task<TEntity> Get(Guid id);
        Task<IEnumerable<TEntity>> GetAll();
        Task<IEnumerable<TEntity>> GetAllBy(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetFirstBy(Expression<Func<TEntity, bool>> predicate);
        Task Add(TEntity obj);             
        Task Delete(Guid id);
        Task DeletePermanently(Guid id);
        Task Update(TEntity obj);
    }
}
