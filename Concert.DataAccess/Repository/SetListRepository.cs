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
    public class SetListRepository : Repository<SetList>, ISetListRepository
    {
        private ApplicationDbContext _db;

        public SetListRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(SetList setList)
        {
            _db.SetLists.Update(setList);
        }
    }
}