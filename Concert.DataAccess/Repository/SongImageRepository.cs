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
    public class SongImageRepository : Repository<SongImage>, ISongImageRepository
    {
        private ApplicationDbContext _db;

        public SongImageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(SongImage songImage)
        {
            _db.SongImages.Update(songImage);
        }
    }
}