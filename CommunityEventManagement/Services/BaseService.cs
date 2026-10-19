using CommunityEventManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventManagement.Services
{
    // ABSTRACTION + INHERITANCE + GENERICS + Factory/DI.
    // Provides generic CRUD for any entity type T using the DbContext factory.
    public abstract class BaseService<T>(IDbContextFactory<ApplicationDbContext> contextFactory)
        : IService<T> where T : class
    {
        protected readonly IDbContextFactory<ApplicationDbContext> _contextFactory = contextFactory;

        public virtual List<T> GetList()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Set<T>().ToList();
        }

        public virtual T? GetById(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Set<T>().Find(id);
        }

        public virtual void Add(T record)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Set<T>().Add(record);
            context.SaveChanges();
        }

        public virtual void Update(T record)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Entry(record).State = EntityState.Modified;
            context.SaveChanges();
        }

        public virtual void Delete(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var record = context.Set<T>().Find(id);
            if (record != null)
            {
                context.Set<T>().Remove(record);
                context.SaveChanges();
            }
        }
    }
}