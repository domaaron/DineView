using DineView.Application.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineView.Application.infrastructure.Repositories
{
    public abstract class Repository<Tentity, Tkey> where Tentity : class, IEntity<Tkey> where Tkey : struct
    {
        protected readonly DineContext _db;
        public IQueryable<Tentity> Set => _db.Set<Tentity>();
        protected Repository(DineContext db)
        {
            _db = db;
        }

        public Tentity? FindById(Guid guid) => _db.Set<Tentity>().FirstOrDefault(e => e.Id.Equals(guid));
        public Tentity? FindByGuid(Guid guid) => _db.Set<Tentity>().FirstOrDefault(e => e.Guid.Equals(guid));
        public virtual (bool success, string message) Insert(Tentity entity)
        {
            _db.Entry(entity).State = EntityState.Added;
            
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }

            return (true, string.Empty);
        }

        public virtual (bool success, string message) Update(Tentity entity)
        {
            if (entity.Id.Equals(default))
            {
                return (false, "Missing primary key");
            }

            _db.Entry(entity).State = EntityState.Modified;

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }

            return (true, string.Empty);
        }

        public virtual (bool success, string message) Delete(Tentity entity)
        {
            if (entity.Id.Equals(default))
            {
                return (false, "Missing primary key");
            }

            _db.Entry(entity).State = EntityState.Deleted;

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }

            return (true, string.Empty);
        }
    }
}
