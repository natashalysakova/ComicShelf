using AutoMapper;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Services.ViewModels;
using System.Linq.Expressions;

namespace Services.Services
{
    public abstract class BasicService<T, VM, CM, UM> : IService 
        where T : class, IIdEntity 
        where VM : IViewModel
        where CM : ICreateModel
        where UM : IUpdateModel
    {
        protected DbSet<T> dbSet;
        private ComicShelfContext context;
        protected readonly IMapper _mapper;

        public BasicService(ComicShelfContext context, IMapper mapper)
        {
            this.context = context;
            _mapper = mapper;
            dbSet = context.Set<T>();
        }

        public VM Get(int? id)
        {
            return _mapper.Map<VM>(GetById(id));
        }

        public virtual int Add(CM item)
        {
            var model = _mapper.Map<T>(item);
            Add(model);
            return model.Id;
        }

        public virtual void Update(UM item)
        {
            var model = _mapper.Map<T>(item);

            var tracked = GetById(model.Id);
            if (tracked != null)
            {
                Detach(tracked);
            }

            Update(model);
        }
        public virtual IEnumerable<VM> GetAll()
        {
            return _mapper.ProjectTo<VM>(dbSet.AsNoTracking()).ToList();
        }

        public virtual IEnumerable<UM> GetAllForEdit()
        {
            return _mapper.ProjectTo<UM>(dbSet.AsNoTracking());
        }

        protected IQueryable<T> GetAllEntities(bool tracking = false)
        {
            return tracking ? dbSet : dbSet.AsNoTracking();
        }

        internal virtual T? GetById(int? id)
        {
            return dbSet.Find(id);
        }

        public virtual void Remove(T item)
        {
            dbSet.Remove(item);
            context.SaveChanges();
        }
        public virtual void Remove(int? id)
        {
            var item = dbSet.Find(id);
            if(item != null)
            {
                dbSet.Remove(item);
                context.SaveChanges();
            }
        }

        protected void Detach(T item)
        {
            dbSet.Entry(item).State = EntityState.Detached;
        }


        internal void Update(T country)
        {
            dbSet.Entry(country).State = EntityState.Modified;
            context.SaveChanges();
        }


        internal int Add(T item)
        {
            dbSet.Add(item);
            context.SaveChanges();
            return item.Id;
        }

        internal virtual IEnumerable<int> AddRange(IEnumerable<T> items)
        {
            dbSet.AddRange(items);
            context.SaveChanges();
            return items.Select(x => x.Id);
        }

        public bool Exists(int id)
        {
            return GetById(id) != null;
        }

        internal void LoadReference(T item, string reference)
        {
            dbSet.Entry(item).Reference(reference).Load();
        }
        internal void LoadReference<TProperty>(T item, Expression<Func<T, TProperty?>> expression) where TProperty : class
        {
            dbSet.Entry(item).Reference(expression).Load();
        }
        internal void LoadCollection(T item, string collection)
        {
            dbSet.Entry(item).Collection(collection).Load();
        }
        internal void LoadCollection<TProperty>(T item, Expression<Func<T, IEnumerable<TProperty>>> expression) where TProperty : class
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