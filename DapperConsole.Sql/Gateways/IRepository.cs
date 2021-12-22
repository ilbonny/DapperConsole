using System.Collections.Generic;
using System.Threading.Tasks;

namespace DapperConsole.Sql.Gateways
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> Get(int id);
        Task<IEnumerable<TEntity>> GetAll();
        Task Insert(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TEntity entity);
    }
}
