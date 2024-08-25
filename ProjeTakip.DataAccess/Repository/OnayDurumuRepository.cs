using Microsoft.EntityFrameworkCore;
using ProjeTakip.DataAccess.Data;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProjeTakip.DataAccess.Data;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;
using System.Linq;

namespace ProjeTakip.DataAccess.Repository
{
    public class OnayDurumuRepository : Repository<OnayDurumu>, IOnayDurumuRepository
    {
        private readonly ProjeDbContext _context;

        public OnayDurumuRepository(ProjeDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OnayDurumu onayDurumu)
        {
            var objFromDb = _context.OnayDurumu.FirstOrDefault(c => c.OnayDurumuId == onayDurumu.OnayDurumuId);
            if (objFromDb != null)
            {
                objFromDb.OnayDurumuAdi = onayDurumu.OnayDurumuAdi;
                _context.SaveChanges();
            }
        }
    }
}
