namespace CommunityEventManagement.Services
{
    // Generic service contract (the module's Week 11 approach).
    public interface IService<T> where T : class
    {
        List<T> GetList();
        T? GetById(int id);
        void Add(T record);
        void Update(T record);
        void Delete(int id);
    }
}