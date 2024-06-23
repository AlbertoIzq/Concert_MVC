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
    public class SetListSongRepository : Repository<SetListSong>, ISetListSongRepository
    {
        private ApplicationDbContext _db;

        public SetListSongRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(SetListSong setListSong)
        {
            _db.SetListSongs.Update(setListSong);
        }
    }
}