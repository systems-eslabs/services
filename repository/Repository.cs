using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class Repository<T> where T : class
    {
        DbSet<T> _dbset;
        public DbContext _dbcontext;

        public Repository(DbContext _dbcontext)
        {
            this._dbcontext = _dbcontext;
            _dbset = _dbcontext.Set<T>();
        }

        public virtual T Add(T entity)
        {
            _dbset.Add(entity);
            _dbcontext.SaveChanges();
            return entity;
        }

        public virtual void AddRange(List<T> entity)
        {
            _dbset.AddRange(entity);
            _dbcontext.SaveChanges();
        }

        public virtual T Update(T entity)
        {
            if (this.Find(v => v == entity) == entity)
            {
                _dbset.Attach(entity);
                _dbcontext.Entry(entity).State = EntityState.Modified;
                _dbcontext.SaveChanges();
                return entity;
            }
            else
            {
                return null;
            }
        }
        public virtual bool Delete(T entity)
        {
            if (this.Find(v => v == entity) == entity)
            {
                _dbset.Remove(entity);
                _dbcontext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool IsExists(Expression<Func<T, bool>> match)
        {
             return _dbset.Count(match) > 0;
        }

        public virtual T Find(Expression<Func<T, bool>> match)
        {
            return _dbset.Find(match);
        }

        public virtual IEnumerable<T> GetList(Expression<Func<T, bool>> match)
        {
            return _dbset.Where(match);
        }

        public virtual int GetCount(Expression<Func<T, bool>> match)
        {
             return _dbset.Count(match);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbset.ToList();
        }

        // public void SaveChanges()
        // {
        //     _dbcontext.SaveChanges();
        // }
    }
}
