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
        public IServiceRepository Service { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public ISetListSongRepository SetListSong { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Genre = new GenreRepository(_db);
            Language = new LanguageRepository(_db);
            Song = new SongRepository(_db);
            Service = new ServiceRepository(_db);
            Company = new CompanyRepository(_db);
            SetListSong = new SetListSongRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
