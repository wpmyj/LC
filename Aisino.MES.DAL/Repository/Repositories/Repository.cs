using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;
using System.Text;
using System.Data.Entity;

using LC.DAL.Repository.Helpers;
using LC.DAL.Interfaces;
using System.Configuration;
using System.Data.Entity.Validation;
using LC.DAL.Repository.Interfaces;
using Microsoft.Practices.Unity;
//using LC.Model.StoreProcedure;
using LC.Model;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;

namespace LC.DAL.Repository.Repositories
{
    /// <summary>
    /// The GenericRepository allows generic crud operations to take place in a default DbContext or one specified in the constructor.
    /// </summary>
    /// <typeparam name="T">The Type of entity that will be acted upon</typeparam>
    public class Repository<T> : IGenericRepository<T> where T : class
    {
        private bool _Validate;
        public bool Validate
        {
            get
            {
                return _Validate;
            }
            set
            {
                if (this.ctx != null)
                {
                    _Validate = value;
                    this.ctx.Configuration.ValidateOnSaveEnabled = value;
                }
            }
        }

        /// <summary>
        /// the DbContext to use for operations
        /// </summary>
        public DbContext ctx { get; set; }
        /// <summary>
        /// Determines whether to call ctx.SaveChanges() on completion of an action.  Typically, if you are calling this as a standalone
        /// and not part of a unit of work, autosave will be true.  However, if used in a GenericUnitOfWork, then Autosave should be false
        /// to allow the GenericUnitOfWork to perform the save on the context that it passes to this class.
        /// </summary>
        public bool Autosave { get; set; }

        /// <summary>
        /// used to determine if a context should be disposed of during dispose of the repository
        /// </summary>
        private bool AutoDisposeContext;

        /// <summary>
        /// Constructor specifying the DbContext to use for operations, and setting Autosave to false.
        /// </summary>
        /// <param name="context">The DbContext to use for operations</param>
        [InjectionConstructor]
        public Repository(DbContext context)
        {
            ctx = context;
            Autosave = true;
            AutoDisposeContext = true;
        }

        public void RefreshData()
        {
            ObjectContext objectContext = ((IObjectContextAdapter)ctx).ObjectContext;
            objectContext.Refresh(RefreshMode.StoreWins, ctx.Set<T>().Local);
        }

        //private ObjectSet<T> RefreshDataSet()
        //{
        //    ObjectContext objectContext = ((IObjectContextAdapter)ctx).ObjectContext;
        //    ObjectSet<T> set = objectContext.CreateObjectSet<T>();
        //    set.MergeOption = MergeOption.PreserveChanges;
        //    return set;
        //}
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return ctx.Set<T>().Count();
        }

        public RepositoryResultSingle<T> GetObjectByKey(object key)
        {
            RepositoryResultSingle<T> result = new RepositoryResultSingle<T>();

            try
            {
                result.Entity = this.ctx.Set<T>().Find(key);
                result.NoErrors = true;
            }
            catch (Exception ex)
            {
                result.NoErrors = false;
                result.Message = ex.ToString();
            }

            return result;
        }
        /// <summary>
        /// Gets All items of Type T from the data store
        /// </summary>
        /// <returns>IEnumerable of the specified Type/></returns>
        public RepositoryResultList<T> GetAll()
        {
            RepositoryResultList<T> result = new RepositoryResultList<T>();

            try
            {
                result.Entities = ctx.Set<T>().AsEnumerable();
                result.NoErrors = true;
            }
            catch (Exception ex)
            {
                result.NoErrors = false;
                result.Message = ex.ToString();
            }

            return result;
        }

        /// <summary>
        /// Gets all of T from the data store
        /// </summary>
        /// <param name="includes">A list of property names to return in the object graph</param>
        /// <returns>RepositoryReslutList</returns>
        public RepositoryResultList<T> GetAll(params string[] includes)
        {
            RepositoryResultList<T> result = new RepositoryResultList<T>();

            try
            {
                var query = ctx.Set<T>().Where(t => true);
                foreach (string include in includes)
                {
                    query = (IQueryable<T>)query.Include(include);
                }
                result.Entities = query.AsEnumerable();
                result.NoErrors = true;
            }
            catch (Exception ex)
            {
                result.NoErrors = false;
                result.Message = ex.ToString();
            }

            return result;
        }
        /// <summary>
        /// Gets all of T from the data store
        /// </summary>
        /// <param name="paging">paging metadata for the resulting entity collection</param>
        /// <returns>RepositoryResultList</returns>
        public RepositoryResultList<T> GetAll(PagingCriteria paging)
        {
            RepositoryResultList<T> result = new RepositoryResultList<T>();


            try
            {
                var data = ctx.Set<T>().Where(t => true);
                int totalRecords = data.Count();

                result.Entities = data.AsQueryable()
                    .OrderBy(paging.SortBy + " " + paging.SortDirection)
                    .Skip((paging.PageNumber - 1) * paging.PageSize)
                    .Take(paging.PageSize)
                    .AsEnumerable();

                result.PagedMetadata = new PagedMetadata(totalRecords, paging.PageSize, paging.PageNumber);
                result.NoErrors = true;
            }
            catch (Exception ex)
            {
                result.NoErrors = false;
                result.Message = ex.ToString();
            }

            return result;
        }
        /// <summary>
        /// Gets all of T from the data store
        /// </summary>
        /// <param name="paging">pagingcriteria to determine which page of T to return</param>
        /// <param name="includes">a parameter string array of property names to return in each item of the entity object graph</param>
        /// <returns>RepositoryResultList</returns>
        public RepositoryResultList<T> GetAll(PagingCriteria paging, params string[] includes)
        {
            RepositoryResultList<T> result = new RepositoryResultList<T>();

            try
            {
                var data = ctx.Set<T>().Where(t => true);
                int totalRecords = data.Count();

                foreach (string include in includes)
                {
                    data = (IQueryable<T>)data.Include(include);
                }

                result.Entities = data.AsQueryable()
                    .OrderBy(paging.SortBy + " " + paging.SortDirection)
                    .Skip((paging.PageNumber - 1) * paging.PageSize)
                    .Take(paging.PageSize)
                    .AsEnumerable();

                result.PagedMetadata = new PagedMetadata(totalRecords, paging.PageSize, paging.PageNumber);
                result.NoErrors = true;
            }
            catch (Exception ex)
            {
                result.NoErrors = false;
                result.Message = ex.ToString();
            }

            return result;
        }

        /// <summary>
        /// Performs a find on the repository given the specified Type and Filter expression (this will come from the FilterStore.FilterForUser
        /// method or a simple lambda expression
        /// </summary>
        /// <param name="filter">the Expression to use to filter objects from the data store</param>
        /// <returns>RepositoryResultList</returns>
        public RepositoryResultList<T> Find(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            RepositoryResultList<T> result = new RepositoryResultList<T>();

            try
            {
                result.Entities = ctx.Set<T>().Where(filter.Compile());
                result.NoErrors = true;
            }
            catch (Exception ex)
            {
                result.NoErrors = false;
                result.Message = ex.ToString();
            }

            return result;
        }
        /// <summary>
        /// Performs a find on the repository given the specified Type and Filter expression (this will come from the FilterStore.FilterForUser
        /// method or a simple lambda expression
        /// </summary>
        /// <param name="filter">the Expression to use to filter objects from the data store</param>
        /// <param name="includes">a params string array of property names that will be included in the resulting object graph for each item</param>
        /// <returns>RepositoryResultList</returns>
        public RepositoryResultList<T> Find(System.Linq.Expressions.Expression<Func<T, bool>> filter, params string[] includes)
        {
            RepositoryResultList<T> result = new RepositoryResultList<T>();

            try
            {
                var query = ctx.Set<T>().Where(t => true);
                foreach (string include in includes)
                {
                    query = query.Include(include);
                }
                result.Entities = query.Where(filter.Compile());
                result.NoErrors = true;
            }
            catch (Exception ex)
            {
                result.NoErrors = false;
                result.Message = ex.ToString();
            }

            return result;
        }
        /// <summary>
        /// Performs a find on the repository given the specified Type and Filter expression (this will come from the FilterStore.FilterForUser
        /// method or a simple lambda expression
        /// </summary>
        /// <param name="filter">the Expression to use to filter objects from the data store</param>
        /// <param name="paging">PagingCriteria for the result list</param>
        /// <returns>RepositoryResultList</returns>
        public RepositoryResultList<T> Find(Expression<Func<T, bool>> filter, PagingCriteria paging)
        {
            RepositoryResultList<T> result = new RepositoryResultList<T>();

            try
            {
                var data = ctx.Set<T>().Where(filter.Compile());
                int totalRecords = data.Count();

                result.Entities = data.AsQueryable()
                    .OrderBy(paging.SortBy + " " + paging.SortDirection)
                    .Skip((paging.PageNumber - 1) * paging.PageSize)
                    .Take(paging.PageSize)
                    .AsEnumerable();

                result.PagedMetadata = new PagedMetadata(totalRecords, paging.PageSize, paging.PageNumber);
                result.NoErrors = true;
            }
            catch (Exception ex)
            {
                result.NoErrors = false;
                result.Message = ex.ToString();
            }

            return result;
        }
        /// <summary>
        /// Performs a find on the repository given the specified Type and Filter expression (this will come from the FilterStore.FilterForUser
        /// method or a simple lambda expression
        /// </summary>
        /// <param name="filter">the Expression to use to filter objects from the data store</param>
        /// <param name="paging">PagingCriteria for the result list</param>
        /// <param name="includes">a params string array of property names that will be included in the resulting object graph for each item</param>
        /// <returns>RepositoryResultList</returns>
        public RepositoryResultList<T> Find(Expression<Func<T, bool>> filter, PagingCriteria paging, params string[] includes)
        {
            RepositoryResultList<T> result = new RepositoryResultList<T>();

            try
            {
                var query = ctx.Set<T>().Where(t => true);
                foreach (string include in includes)
                {
                    query = query.Include(include);
                }

                query = query.Where(filter.Compile()).AsQueryable();
                int totalRecords = query.Count();
                result.Entities = query.AsQueryable()
                    .OrderBy(paging.SortBy + " " + paging.SortDirection)
                    .Skip((paging.PageNumber - 1) * paging.PageSize)
                    .Take(paging.PageSize)
                    .AsEnumerable();

                result.PagedMetadata = new PagedMetadata(totalRecords, paging.PageSize, paging.PageNumber);
                result.NoErrors = true;
            }
            catch (Exception ex)
            {
                result.NoErrors = false;
                result.Message = ex.ToString();
            }

            return result;
        }
        /// <summary>
        /// Returns a single entity matching the filter provided
        /// </summary>
        /// <param name="filter">The Expression used to filter objects from the data store</param>
        /// <returns>RepositoryResultList</returns>
        public RepositoryResultSingle<T> Single(Expression<Func<T, bool>> filter)
        {
            RepositoryResultSingle<T> result = new RepositoryResultSingle<T>();

            try
            {
                result.Entity = ctx.Set<T>().SingleOrDefault(filter.Compile());
                result.NoErrors = true;
            }
            catch (Exception ex)
            {
                result.NoErrors = false;
                result.Message = ex.ToString();
            }

            return result;
        }

        public RepositoryResultSingle<T> SingleAsNoTracking(Expression<Func<T, bool>> filter)
        {
            RepositoryResultSingle<T> result = new RepositoryResultSingle<T>();

            try
            {
                result.Entity = ctx.Set<T>().AsNoTracking().SingleOrDefault(filter);
                result.NoErrors = true;
            }
            catch (Exception ex)
            {
                result.NoErrors = false;
                result.Message = ex.ToString();
            }

            return result;
        }
        /// <summary>
        /// Returns a single entity matching the filter provided
        /// </summary>
        /// <param name="filter">The Expression used to filter objects from the data store</param>
        /// <param name="includes">a parameter list of property names to include in each item in the returned entity object graph</param>
        /// <returns>RepositoryResultList</returns>
        public RepositoryResultSingle<T> Single(Expression<Func<T, bool>> filter, params string[] includes)
        {
            RepositoryResultSingle<T> result = new RepositoryResultSingle<T>();

            try
            {
                var query = ctx.Set<T>().Where(t => true);
                foreach (string include in includes)
                {
                    query = query.Include(include);
                }
                result.Entity = query.SingleOrDefault(filter.Compile());

                result.NoErrors = true;
            }
            catch (Exception ex)
            {
                result.NoErrors = false;
                result.Message = ex.ToString();
            }

            return result;
        }

        /// <summary>
        /// Adds an item of T to the DbContext
        /// </summary>
        /// <param name="item">the item to add</param>
        public void Add(T item)
        {
            try
            {
                var set = ctx.Set<T>();
                set.Add(item);
                ctx.Entry<T>(item).State = System.Data.Entity.EntityState.Added;
                if (Autosave)
                {
                    ctx.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                //RefreshData();
                throw new RepositoryException(ex.Message, ex);
            }
            catch (DbUpdateException ex)
            {
                //RefreshData();
                throw new RepositoryException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                //RefreshData();
                throw new RepositoryException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Attaches and updates the item in the DbContext
        /// </summary>
        /// <param name="item">the item to update</param>
        public void Update(T item)
        {
            try
            {
                var set = ctx.Set<T>();
                set.Attach(item);
                ctx.Entry<T>(item).State = System.Data.Entity.EntityState.Modified;
                if (Autosave)
                {
                    ctx.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                RefreshData();
                throw new RepositoryException(ex.Message, ex);
            }
            catch (DbUpdateException ex)
            {
                RefreshData();
                throw new RepositoryException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                RefreshData();
                throw new RepositoryException(ex.Message, ex);
            }
        }
        /// <summary>
        /// Deletes the item from the DbContext
        /// </summary>
        /// <param name="item">the item to delete</param>
        public void Delete(T item)
        {

            try
            {
                var set = ctx.Set<T>();
                T existing = set.Attach(item);
                if (existing != null)
                {
                    ctx.Set<T>().Remove(existing);
                }
                if (Autosave)
                {
                    ctx.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                RefreshData();
                throw new RepositoryException(ex.Message, ex);
            }
            catch (DbUpdateException ex)
            {
                RefreshData();
                throw new RepositoryException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                RefreshData();
                throw new RepositoryException(ex.Message, ex);
            }
        }

        public void DeleteAll(List<T> entitys)
        {
            try
            {
                var set = ctx.Set<T>();
                foreach (T item in entitys)
                {
                    T existing = set.Attach(item);
                    if (existing != null)
                    {
                        ctx.Set<T>().Remove(existing);
                    }
                }
                if (Autosave)
                {
                    ctx.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
        }
        /// <summary>
        /// disposes of the context
        /// </summary>
        public void Dispose()
        {
            if (AutoDisposeContext)
            {
                ctx.Dispose();
            }
        }


        /// <summary>
        /// Gets All items of Type T from the data store
        /// </summary>
        /// <returns>IEnumerable of the specified Type/></returns>
        public IEnumerable<T> GetAllEntity()
        {
            return ctx.Set<T>().AsEnumerable();
        }

        public IEnumerable<T> GetAllEntity(params string[] includes)
        {
            var query = ctx.Set<T>().Where(t => true);
            foreach (string include in includes)
            {
                query = (IQueryable<T>)query.Include(include);
            }
            return query.AsEnumerable();
        }
        /// <summary>
        /// Performs a find on the repository given the specified Type and Filter expression (this will come from the FilterStore.FilterForUser
        /// method or a simple lambda expression
        /// </summary>
        /// <param name="filter">the Expression to use to filter objects from the data store</param>
        /// <returns></returns>
        public IEnumerable<T> FindEntity(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            return ctx.Set<T>().Where(filter.Compile()).AsEnumerable();
        }
        /// <summary>
        /// Performs a find, including sub objects
        /// </summary>
        /// <param name="filter">The Expression to use to filter the entities</param>
        /// <param name="includes">the sub-objects to include (by property name)</param>
        /// <returns></returns>
        public IEnumerable<T> FindEntity(System.Linq.Expressions.Expression<Func<T, bool>> filter, params string[] includes)
        {
            var query = ctx.Set<T>().Where(t => true);
            foreach (string include in includes)
            {
                query = query.Include(include);
            }
            return query.Where(filter.Compile());

        }
        /// <summary>
        /// returns a single entity based on the expression
        /// </summary>
        /// <param name="filter">the expression used to filter to a single entity</param>
        /// <returns>T</returns>
        T IGenericRepository<T>.SingleEntity(Expression<Func<T, bool>> filter)
        {
            return ctx.Set<T>().SingleOrDefault(filter);
        }
        /// <summary>
        /// returns a single entity based on the expression
        /// </summary>
        /// <param name="filter">the expression used to filter to a single entity</param>
        /// <param name="includes">the sub objects to include (by property name)</param>
        /// <returns>T</returns>
        T IGenericRepository<T>.SingleEntity(Expression<Func<T, bool>> filter, params string[] includes)
        {
            var query = ctx.Set<T>().Where(filter);
            foreach (string include in includes)
            {
                query = query.Include(include);
            }
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 获得存储过程结果集
        /// </summary>
        /// <param name="storedProc"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        //public ResultsList CallStoredProc(StoredProc<T> storedProc, T parm)
        //{
        //    //ResultsList results = ctx.CallStoredProc<T>(storedProc, parm);
        //    //return results;
        //    return null;
        //}

        public RepositoryResultList<T> QueryByESql(string esql)
        {
            RepositoryResultList<T> result = new RepositoryResultList<T>();

            try
            {
                this.RefreshData();
                result.Entities = ctx.Set<T>().SqlQuery(esql).ToList();
                result.NoErrors = true;
            }
            catch (Exception ex)
            {
                result.NoErrors = false;
                result.Message = ex.ToString();
            }

            return result;
        }

        public IEnumerable<T> QueryCustomerObjectByESql(string esql)
        {
            this.RefreshData();
            return ctx.Database.SqlQuery<T>(esql);
        }

        public RepositoryResultList<T> QueryByESql(string esql, PagingCriteria paging)
        {
            RepositoryResultList<T> result = new RepositoryResultList<T>();

            try
            {
                this.RefreshData();
                var data = ctx.Set<T>().SqlQuery(esql);
                int totalRecords = data.Count();

                result.Entities = data.AsQueryable()
                    .OrderBy(paging.SortBy + " " + paging.SortDirection)
                    .Skip((paging.PageNumber - 1) * paging.PageSize)
                    .Take(paging.PageSize)
                    .AsEnumerable();

                result.PagedMetadata = new PagedMetadata(totalRecords, paging.PageSize, paging.PageNumber);
                result.NoErrors = true;

            }
            catch (Exception ex)
            {
                result.NoErrors = false;
                result.Message = ex.ToString();
            }

            return result;
        }

    }
}
