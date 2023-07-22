using ComicShelf.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.Internal.TypeMapping;
using System.Linq;

namespace ComicShelf.Services
{
    public abstract class BasicService<T> : IService where T : class
    {
        protected DbSet<T> dbSet;
        protected ComicShelfContext context;
        public BasicService(ComicShelfContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public virtual IQueryable<T> GetAll()
        {
            return dbSet.AsNoTracking();
        }

        public virtual T? Get(int? id)
        {
            if (id == null)
            {
                return default;
            }
            else
            {
                return dbSet.Find(id);
            }
        }

        public virtual void Remove(T item)
        {
            dbSet.Remove(item);
            context.SaveChanges();
        }

        public virtual void Update(T country)
        {
            dbSet.Entry(country).State = EntityState.Modified;
            context.SaveChanges();
        }

        public virtual void Add(T item)
        {
            dbSet.Add(item);
            context.SaveChanges();
        }

        public virtual void AddRange(IEnumerable<T> items)
        {
            dbSet.AddRange(items);
            context.SaveChanges();
        }

        internal bool Exists(int id)
        {
            return Get(id) != null;
        }
    }
}