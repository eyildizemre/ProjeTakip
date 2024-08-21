using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.DataAccess.Repository.IRepository
{
	public interface IRepository<T> where T : class
	{
		IEnumerable<T> GetAll();
		T Get(object primaryKey);
		void Add(T entity);
		void Remove(object primaryKey);
		void Remove(T entity);
	}
}
