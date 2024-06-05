using Concert.DataAccess.Data;
using Concert.DataAccess.Repository.IRepository;
using Concert.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Concert.DataAccess.Repository
{
    public class LanguageRepository : Repository<Language>, ILanguageRepository
    {
        private ApplicationDbContext _db;

        public LanguageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Language language)
        {
            _db.Languages.Update(language);
        }
    }
}