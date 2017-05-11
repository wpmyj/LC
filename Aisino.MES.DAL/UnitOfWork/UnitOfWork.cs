using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

using System.Configuration;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Transactions;
using Microsoft.Practices.Unity;
using LC.DAL.Repository.Interfaces;
using LC.DAL.Enums;
using LC.DAL.Repository;
using LC.DAL.Interfaces;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;

namespace LC.DAL.UnitOfWork
{
    /// <summary>
    /// For items that require more than single entity operations, the GenericUnitOfWork provides a way
    /// to chain DataActions together to perform work on the same context.  A typical interaction would be to
    /// create the GenericUnitOfWork, add IDataActions to it and the call the Save method (which performs the
    /// actions in the order they were added all on the same context, and then call SaveChanges on the context
    /// (making the GenericRepositories pass-through the context).
    /// </summary>
    public class UnitOfWork : IDisposable
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(UnitOfWork));
        private bool Validate { get; set; }
        /// <summary>
        /// The list of actions to perform
        /// </summary>
        public List<IDataAction> Actions { get; set; }
        /// <summary>
        /// the context in which to perform DataActions specfied in the Actions list
        /// </summary>
        public DbContext Context { get; set; }

        public TransactionScope TransactionScope { get; set; }

        private Dictionary<Type, object> _repositoryDic = new Dictionary<Type, object>();

        /// <summary>
        /// Constructor specifying the DbContext to use
        /// </summary>
        /// <param name="ctx">The DbContext to use</param>
        [InjectionConstructor]
        public UnitOfWork(DbContext ctx)
        {
            Context = ctx;
            Context.Configuration.ValidateOnSaveEnabled = true;
            Context.Configuration.ProxyCreationEnabled = true;
            Context.Configuration.AutoDetectChangesEnabled = true;
            Context.Configuration.LazyLoadingEnabled = true;
            Context.Database.Log = (log) => { Debug.WriteLine(log); };
            Actions = new List<IDataAction>();
        }

        public LC.DAL.Repository.Repositories.Repository<T> GetRepository<T>() where T : class
        {
            if (!_repositoryDic.Keys.Contains(typeof(T)))
            {
                var repository = new LC.DAL.Repository.Repositories.Repository<T>(Context);
                try
                {
                    _repositoryDic.Add(typeof(T), repository);
                }
                catch (ArgumentException)
                {
                    // Do nothing. The Try logic is to make sure thread safe.
                }
            }

            return _repositoryDic[typeof(T)] as LC.DAL.Repository.Repositories.Repository<T>;
        }
        
        /// <summary>
        /// Adds DataAction to the Actions list
        /// </summary>
        /// <typeparam name="T">The Type of entity</typeparam>
        /// <param name="entity">The actual entity on which to perform the action</param>
        /// <param name="action">The action to perform</param>
        public void AddAction<T>(T entity, DataActions action) where T : class
        {
            if (Context != null)
            {
                DataAction<T> act = new DataAction<T>(Context, entity, action);
                this.Actions.Add(act);
            }
        }
        /// <summary>
        /// Peforms all of the actions specified in the Actions list in the order they were added and calls SaveChanges() on the context
        /// we assume that this.TransactionScope will be part of a using external to this object.  If you pass it in, it's up to you to
        /// dispose of it.
        /// </summary>
        public void Save()
        {
            TransactionScope transaction = this.TransactionScope ?? new TransactionScope();
            
            try
            {
                //Context.Configuration.AutoDetectChangesEnabled = false;
                foreach (var action in Actions)
                {
                    action.Save();
                }
                if (this.Context != null)
                {
                    Context.SaveChanges();
                }
                transaction.Complete();
            }
            catch (DbEntityValidationException ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message, ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
            finally 
            {
                if (this.TransactionScope == null)
                {
                    transaction.Dispose();
                }
                if (Actions != null && Actions.Count > 0)
                {
                    //单例模式时必须清空
                    Actions.Clear();
                }
                //Context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        #region Dispose

        private bool _disposed = false;

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            log.Info(String.Format("dispose unitofwork{0}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing && Context != null)
                {
                    // Dispose managed resources.
                    Context.Dispose();
                    Context = null;
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                // If disposing is false, 
                // only the following code is executed.
            }

            this._disposed = true;
        }

        #endregion
    }
}
