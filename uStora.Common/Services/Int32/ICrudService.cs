namespace uStora.Common.Services.Int32
{
    public interface ICrudService<T> where T : class
    {
        T Add(T entity);
        void Update(T entity);
        void Delete(int id);
        void SaveChanges();

    }
}
