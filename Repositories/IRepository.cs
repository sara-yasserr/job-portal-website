namespace Job_Portal_Project.Repositories
{
    public interface IRepository<T>
    {
        List<T> GetAll();
        T GetById<I>(I id);
        void Insert(T entity);
        void Update(T entity);
        void Delete<I>(I id);
        public int Count();
        void Save();

    }
}
