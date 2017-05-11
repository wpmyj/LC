using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Configuration;

using System.Transactions;
using LC.DAL.Repository.Interfaces;
using LC.DAL.Repository.Repositories;
using LC.DAL.Enums;

namespace LC.DAL.Repository
{
    /// <summary>
    /// Describes a data action that must be performed.  It uses a <see cref="GenericRepository"/>, an entity and a DbContext to perform the action.
    /// </summary>
    /// <typeparam name="T">The type of entity, T must be a class and implement the IHasId inteface</typeparam>
    public class DataAction<T> : IDataAction where T : class
    {
        /// <summary>
        /// The repository that will perform the action
        /// </summary>
        public Repository<T> Repository { get; set; }
        /// <summary>
        /// The entity on which the action will be performed
        /// </summary>
        public T Entity { get; set; }
        /// <summary>
        /// The context in which the action will be performed
        /// </summary>
        public DbContext Context { get; set; }
        /// <summary>
        /// The action to perform on the entity
        /// </summary>
        public DataActions Action { get; set; }

        /// <summary>
        /// Constructor specifying the context in which the action will be performed (otherwise it will create a new default context for use in the repository)
        /// </summary>
        /// <param name="ctx"></param>
        public DataAction(DbContext ctx)
        {
            Context = ctx;
            Repository = new Repository<T>(Context) { Autosave = false };
        }
        
        /// <summary>
        /// Constructor specifying the context and entity on which to act
        /// </summary>
        /// <param name="ctx">The DbContext to use</param>
        /// <param name="entity">The entity on which to perform the action</param>
        public DataAction(DbContext ctx, T entity)
        {
            Context = ctx;
            Repository = new Repository<T>(Context) { Autosave = false };
            Entity = entity;
        }
        /// <summary>
        /// Constructor specifying the context, entity and action
        /// </summary>
        /// <param name="ctx">The DbContext to use</param>
        /// <param name="entity">The entity on which to perform the action</param>
        /// <param name="action">The action to perform</param>
        public DataAction(DbContext ctx, T entity, DataActions action)
        {
            Context = ctx;
            Repository = new Repository<T>(Context) { Autosave = false };
            Entity = entity;
            Action = action;
        }
        /// <summary>
        /// Adds the entity to the context using the repository
        /// </summary>
        private void Add()
        {
            Repository.Add(Entity);
        }
        /// <summary>
        /// Updates the entity in the context using the repository
        /// </summary>
        private void Update()
        {
            Repository.Update(Entity);
        }
        /// <summary>
        /// Deletes the entity in the context using the repository
        /// </summary>
        private void Delete()
        {
            Repository.Delete(Entity);
        }
        /// <summary>
        /// Performs the Action on the Entity in the Context using the Repository
        /// </summary>
        public void Save()
        {
            
            switch (Action)
            {
                case DataActions.Add:
                    Add();
                    break;
                case DataActions.Update:
                    Update();
                    break;
                case DataActions.Delete:
                    Delete();
                    break;
                default:
                    break;
            }
        }
    }
}
