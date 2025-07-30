using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using KC.Framework.Base;
using KC.Framework.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace KC.Database.Extension
{
    public static class DbContextExtensions
    {

        public static object[] GetEntityKey<T>(this DbContext context, T entity) where T : class
        {
            var state = context.Entry(entity);
            var metadata = state.Metadata;
            var key = metadata.FindPrimaryKey();
            var props = key.Properties.ToArray();

            return props.Select(x => x.GetGetter().GetClrValue(entity)).ToArray();
        }

        public static TEntity GetOriginalValue<TEntity>(this EntityEntry<TEntity> entry) where TEntity : class
        {
            if (entry.State == EntityState.Detached)
            {
                return entry.Entity;
            }

            var context = entry.Context;
            var entity = entry.Entity;
            var keyValues = context.GetEntityKey(entity);

            entry.State = EntityState.Detached;

            var oEntity = context.Set<TEntity>().Find(keyValues);
            var oEntry = context.Entry(oEntity);

            oEntry.State = EntityState.Detached;
            entry.State = EntityState.Unchanged;
            foreach (var prop in oEntry.Metadata.GetProperties())
            {
                var type = prop.ClrType;
                type = Nullable.GetUnderlyingType(type) ?? type;

                dynamic proposedValue = entry.Property(prop.Name).CurrentValue;
                if (proposedValue != null)
                {
                    proposedValue = Convert.ChangeType(proposedValue, type);
                }
                dynamic originalValue = oEntry.Property(prop.Name).CurrentValue;
                if (originalValue != null)
                {
                    originalValue = Convert.ChangeType(originalValue, type);
                }

                if (proposedValue != originalValue)
                {
                    //changedProps.Add(prop.Name);
                    entry.Property(prop.Name).OriginalValue = oEntry.Property(prop.Name).CurrentValue;
                    entry.Property(prop.Name).IsModified = true;
                }
            }
            return entry.Entity;
        }

        public static TEntity Reload<TEntity>(this DbContext context, TEntity entity) where TEntity : class
        {
            return context.Entry(entity).Reload<TEntity>();
        }

        public static TEntity Reload<TEntity>(this EntityEntry<TEntity> entry) where TEntity : class
        {
            if (entry.State == EntityState.Detached)
            {
                return entry.Entity;
            }

            var context = entry.Context;
            var entity = entry.Entity;
            var keyValues = context.GetEntityKey(entity);

            entry.State = EntityState.Detached;

            var newEntity = context.Set<TEntity>().Find(keyValues);
            var newEntry = context.Entry(newEntity);

            foreach (var prop in newEntry.Metadata.GetProperties())
            {
                prop.FieldInfo.SetValue(entity, prop.GetGetter().GetClrValue(newEntity));
                //prop.GetSetter().SetClrValue(entity, prop.GetGetter().GetClrValue(newEntity));
            }

            newEntry.State = EntityState.Detached;
            entry.State = EntityState.Unchanged;

            return entry.Entity;
        }

        public static IEnumerable<EntityEntry<TEntity>> Local<TEntity>(this DbSet<TEntity> set, params object[] keyValues) where TEntity : class
        {
            var context = set.GetInfrastructure<IServiceProvider>().GetService<DbContext>();
            var entries = context.ChangeTracker.Entries<TEntity>();

            if (keyValues.Any() == true)
            {
                var entityType = context.Model.FindEntityType(typeof(TEntity));
                var keys = entityType.GetKeys();
                var i = 0;

                foreach (var property in keys.SelectMany(x => x.Properties))
                {
                    var keyValue = keyValues[i];
                    entries = entries.Where(e => keyValue.Equals(e.Property(property.Name).CurrentValue));
                    i++;
                }
            }

            return entries;
        }

        public static void Evict<TEntity>(this DbContext context, TEntity entity) where TEntity : class
        {
            context.Entry(entity).State = EntityState.Detached;
        }

        public static void Evict<TEntity>(this DbContext context, params object[] keyValues) where TEntity : class
        {
            var tracker = context.ChangeTracker;
            var entries = tracker.Entries<TEntity>();

            if (keyValues.Any() == true)
            {
                var entityType = context.Model.FindEntityType(typeof(TEntity));
                var keys = entityType.GetKeys();

                var i = 0;

                foreach (var property in keys.SelectMany(x => x.Properties))
                {
                    var keyValue = keyValues[i];

                    entries = entries.Where(e => keyValue.Equals(e.Property(property.Name).CurrentValue));

                    i++;
                }
            }

            foreach (var entry in entries.ToList())
            {
                entry.State = EntityState.Detached;
            }
        }

        public static void AddOrUpdate<TEntity>(this DbContext context, TEntity entity, bool isSave = true) where TEntity : class
        {
            //var entry = context.Entry(entity).GetOriginalValue<TEntity>();
            try
            {
                var entry = context.Entry(entity);
                var keyValues = context.GetEntityKey(entity);
                var oEntity = context.Set<TEntity>().Find(keyValues);
                if (oEntity == null)
                {
                    entry.State = EntityState.Added;
                    context.Set<TEntity>().Add(entity);

                    if (isSave)
                        context.SaveChanges();
                }
                else
                {
                    var oEntry = context.Entry(oEntity);
                    oEntry.State = EntityState.Detached;

                    oEntry.State = EntityState.Detached;
                    entry.State = EntityState.Unchanged;

                    foreach (var prop in oEntry.Metadata.GetProperties())
                    {
                        var type = prop.ClrType;
                        type = Nullable.GetUnderlyingType(type) ?? type;

                        dynamic proposedValue = entry.Property(prop.Name).CurrentValue;
                        if (proposedValue != null)
                        {
                            proposedValue = Convert.ChangeType(proposedValue, type);
                        }
                        dynamic originalValue = oEntry.Property(prop.Name).CurrentValue;
                        if (originalValue != null)
                        {
                            originalValue = Convert.ChangeType(originalValue, type);
                        }

                        if (proposedValue != originalValue)
                        {
                            //changedProps.Add(prop.Name);
                            entry.Property(prop.Name).OriginalValue = oEntry.Property(prop.Name).CurrentValue;
                            entry.Property(prop.Name).IsModified = true;
                        }
                    }

                    if (isSave)
                        context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogFatal(string.Format("--AddOrUpdate Entity is failed. \r\nMessage: {0}. \r\nStackTrace: {1}", ex.Message, ex.StackTrace));
                if(ex.InnerException != null)
                    LogUtil.LogFatal(string.Format("--AddOrUpdate Entity is failed. \r\nInnerMessage: {0}. \r\nInnerStackTrace: {1}", ex.InnerException.Message, ex.InnerException.StackTrace));
            }
        }

        public static void AddOrUpdates<TEntity>(this DbContext context, List<TEntity> entities, bool isSave = true) where TEntity : class
        {
            foreach(var enttiy in entities)
            {
                AddOrUpdate(context, enttiy, isSave);
            }
        }

        public static void Detach<T>(this DbContext context, T entity) where T : class
        {
            try
            {
                if (context.Entry(entity).State == EntityState.Detached)
                    return;

                context.Entry(entity).State = EntityState.Detached;
                
                //context.Detach<T>(entity);

                //var objContext = ((IObjectContextAdapter)context).ObjectContext;
                //var objSet = objContext.CreateObjectSet<T>();
                //var entityKey = objContext.CreateEntityKey(objSet.EntitySet.Name, entity);

                //Object foundEntity;
                //var exists = objContext.TryGetObjectByKey(entityKey, out foundEntity);

                //if (exists)
                //{
                //    objContext.Detach(foundEntity);
                //}
            }
            catch (Exception)
            {
            }
        }

        //public static void AdapterSaveChanges(this System.Data.Entity.DbContext context)
        //{
        //    try
        //    {
        //        ((System.Data.Entity.Infrastructure.IObjectContextAdapter) context).ObjectContext.SaveChanges();
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}
    }

}
