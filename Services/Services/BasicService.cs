using AutoMapper;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Services.ViewModels;
using System.Linq.Expressions;

namespace Services.Services
{
    public abstract class BasicService<T, VM, CM, UM> : IService
        where T : class, IIdEntity
        where VM : IViewModel<T>
        where CM : ICreateModel<T>
        where UM : IUpdateModel<T>
    {
        protected readonly IMapper _mapper;
        protected DbSet<T> dbSet;
        private ComicShelfContext context;
        private string notificationCache;

        public BasicService(ComicShelfContext context, IMapper mapper)
        {
            this.context = context;
            _mapper = mapper;
            dbSet = context.Set<T>();
        }

        public virtual int Add(CM item)
        {
            var model = _mapper.Map<T>(item);
            Add(model);
            return model.Id;
        }

        public bool Exists(int id)
        {
            return GetById(id) != null;
        }

        public VM Get(int? id)
        {
            return _mapper.Map<VM>(GetById(id));
        }
        public virtual IEnumerable<VM> GetAll()
        {
            return _mapper.ProjectTo<VM>(GetAllEntities());
        }

        public IEnumerable<UM> GetAllForUpdate()
        {
            var entries = GetAllEntities();
            return _mapper.ProjectTo<UM>(entries);
        }

        public virtual UM GetForUpdate(int? id)
        {
            var item = GetAllEntities().SingleOrDefault(x => x.Id == id);
            if (item != null)
                return _mapper.Map<UM>(item);
            return default(UM);
        }

        public virtual void Remove(int? id)
        {
            var item = dbSet.Find(id);
            if (item != null)
            {
                dbSet.Remove(item);
                context.SaveChanges();
            }
        }

        public abstract string SetNotificationMessage();

        public string ShowNotification()
        {
            if (string.IsNullOrEmpty(notificationCache))
            {
                notificationCache = SetNotificationMessage();
            }

            return notificationCache;
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
        internal int Add(T item)
        {
            dbSet.Add(item);
            context.SaveChanges();
            return item.Id;
        }

        internal IEnumerable<int> AddRange(IEnumerable<T> items)
        {
            dbSet.AddRange(items);
            context.SaveChanges();
            return items.Select(x => x.Id);
        }

        internal T? GetById(int? id)
        {
            return dbSet.Find(id);
        }

        internal void LoadCollection(T item, string collection)
        {
            dbSet.Entry(item).Collection(collection).Load();
        }

        internal void LoadCollection<TProperty>(T item, Expression<Func<T, IEnumerable<TProperty>>> expression) where TProperty : class
        {
            dbSet.Entry(item).Collection(expression).Load();
        }

        internal void LoadReference(T item, string reference)
        {
            dbSet.Entry(item).Reference(reference).Load();
        }

        internal void LoadReference<TProperty>(T item, Expression<Func<T, TProperty?>> expression) where TProperty : class
        {
            dbSet.Entry(item).Reference(expression).Load();
        }

        internal void Update(T item)
        {
            var existingEntity = context.Set<T>().Local.FirstOrDefault(e => e.Id == item.Id);

            if (existingEntity != null)
            {
                dbSet.Entry(existingEntity).State = EntityState.Detached;
            }

            dbSet.Update(item);

            context.SaveChanges();
        }

        protected void Detach(T item)
        {
            dbSet.Entry(item).State = EntityState.Detached;
        }

        protected virtual IQueryable<T> GetAllEntities(bool tracking = false)
        {
            return tracking ? dbSet : dbSet.AsNoTracking();
        }
    }
}