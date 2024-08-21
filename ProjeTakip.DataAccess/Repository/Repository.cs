using Microsoft.EntityFrameworkCore;
using ProjeTakip.DataAccess.Data;
using ProjeTakip.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.DataAccess.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly ProjeDbContext _context;
		private DbSet<T> _dbSet;

		public Repository(ProjeDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<T>(); // Veritabanı tablosuna erişim
		}

		public IEnumerable<T> GetAll()
		{
			return _dbSet.ToList(); // Tüm kayıtları döndürür
		}

		public T Get(object primaryKey)
		{
			return _dbSet.Find(primaryKey); // Belirli bir ID'ye sahip kaydı döndürür
		}

		public void Add(T entity)
		{
			_dbSet.Add(entity); // Yeni bir kayıt ekler
		}

		public void Remove(object primaryKey)
		{
			var entity = _dbSet.Find(primaryKey); // Doğru primary key alanını bulur
			if (entity != null)
			{
				_dbSet.Remove(entity);
			}
		}

		public void Remove(T entity)
		{
			_dbSet.Remove(entity); // Verilen kaydı siler
		}
	}

}
