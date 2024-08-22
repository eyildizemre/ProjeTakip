using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string? includeProperties = null); // includeProperties parametresi eklendi
        T Get(object primaryKey);
        void Add(T entity);
        void Remove(object primaryKey);
        void Remove(T entity);

        // Yeni eklenen GetFirstOrDefault metodu
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string includeProperties = null);
    }
}
