using Job_Portal_Project.Models;
using Job_Portal_Project.Models.DbContext;

namespace Job_Portal_Project.Repositories
{
    public class FavouritesRepository : IFavouritesRepository
    {
        private JobPortalContext _context;
        public FavouritesRepository (JobPortalContext context)
        {
            _context = context;
        }
        public List<Favourites> GetAll()
        {
            return _context.Favourites.ToList();
        }

        public Favourites GetById<I>(I id)
        {
            return _context.Favourites.Find(id);
        }

        public void Insert(Favourites entity)
        {
            _context.Favourites.Add(entity);
        }

        public void Update(Favourites entity)
        {
            _context.Favourites.Update(entity);
        }

        public void Delete<I>(I id)
        {
            var Record = _context.Favourites.Find(id);
            if (Record != null)
            {
                _context.Favourites.Remove(Record);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

    }
}
