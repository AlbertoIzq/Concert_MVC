using Concert.DataAccess.Data;
using Concert.DataAccess.Repository.IRepository;

namespace Concert.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public IGenreRepository Genre { get; private set; }
        public ILanguageRepository Language { get; private set; }
        public ISongRepository Song { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public ISetListRepository SetList { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Genre = new GenreRepository(_db);
            Language = new LanguageRepository(_db);
            Song = new SongRepository(_db);
            Company = new CompanyRepository(_db);
            SetList = new SetListRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
