using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concert.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        // Inside UnitOfWork we’ll have all the repositories
        IGenreRepository Genre { get; }
        ILanguageRepository Language { get; }
        ISongRepository Song { get; }
        ICompanyRepository Company { get; }
        ISetListRepository SetList { get; }

        // Global methods
        void Save();
    }
}
