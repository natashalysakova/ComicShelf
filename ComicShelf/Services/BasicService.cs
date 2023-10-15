using ComicShelf.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.Internal.TypeMapping;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

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

        public void LoadReference(T item, string reference)
        {
            dbSet.Entry(item).Reference(reference).Load();
        }
        public void LoadReference<TProperty>(T item, Expression<Func<T, TProperty?>> expression) where TProperty : class
        {
            dbSet.Entry(item).Reference(expression).Load();
        }
        public void LoadCollection(T item, string collection)
        {
            dbSet.Entry(item).Collection(collection).Load();
        }
        public void LoadCollection<TProperty>(T item, Expression<Func<T, IEnumerable<TProperty>>> expression) where TProperty : class
        {
            dbSet.Entry(item).Collection(expression).Load();
        }



        string notificationCache;

        public string ShowNotification()
        {
            if (string.IsNullOrEmpty(notificationCache))
            {
                notificationCache = SetNotificationMessage();
            }

            return notificationCache;
        }

        public abstract string SetNotificationMessage();
    }
}