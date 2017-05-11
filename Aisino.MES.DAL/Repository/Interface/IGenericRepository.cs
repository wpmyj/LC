using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using LC.DAL.Repository.Helpers;
using LC.DAL.Repository.Repositories;
//using LC.Model.StoreProcedure;
//using LC.Model.StoreProcedure;


namespace LC.DAL.Repository.Interfaces
{
    /// <summary>
    /// The interface that all of our data repositories have in common.
    /// </summary>
    /// <typeparam name="T">The Type of the repository</typeparam>
    public interface IGenericRepository<T> : IDisposable
    {
        /// <summary>
        /// property to determine whether or not to validate entities on save (in implementation this should default to true)
        /// </summary>
        bool Validate { get; set; }
        /// <summary>
        /// 获得数量
        /// </summary>
        /// <returns></returns>
        int GetCount();
        /// <summary>
        /// 刷新本地缓存数据
        /// </summary>
        void RefreshData();
        /// <summary>
        /// Gets all of a particular entity
        /// </summary>
        /// <returns>RepositoryResultList of matching entities</returns>
        RepositoryResultList<T> GetAll();

        /// <summary>
        /// 根据主键获得
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        RepositoryResultSingle<T> GetObjectByKey(object key);
        /// <summary>
        /// 根据数据库内容读取，并更新缓存
        /// </summary>
        /// <returns></returns>
        //RepositoryResultList<T> ReloadGetAll();
        /// <summary>
        /// Gets all of a particular entity including sub objects defined by the includes
        /// </summary>
        /// <param name="includes">a string array of Property names for which to fetch entities related to the object</param>
        /// <returns>RepositoryResultList of matching entities</returns>
        RepositoryResultList<T> GetAll(params string[] includes);
        /// <summary>
        /// Gets a subset of all of a particular entity based on the PagingCriteria
        /// </summary>
        /// <param name="paging">The PagingCriteria object which defines the page of entities to return</param>
        /// <returns>RepositoryResultList of matching entities</returns>
        RepositoryResultList<T> GetAll(PagingCriteria paging);
        /// <summary>
        /// Gets a subset of all of a particular entity based on the PagingCriteria
        /// </summary>
        /// <param name="paging">The PagingCriteria object which defines the page of entities to return</param>
        /// <param name="includes">a string array of Property names for which to fetch entities related to the object</param>
        /// <returns>RepositoryResultList of matching entities</returns>
        RepositoryResultList<T> GetAll(PagingCriteria paging, params string[] includes);
        
        /// <summary>
        /// Gets a set of entities based on the filter expression
        /// </summary>
        /// <param name="filter">The expression used to filter the entities</param>
        /// <returns>RepositoryResultList of matching entities</returns>
        RepositoryResultList<T> Find(Expression<Func<T, bool>> filter);
        /// <summary>
        /// Gets a set of entities based on the filter expression
        /// </summary>
        /// <param name="filter">The expression used to filter the entities</param>
        /// <param name="includes">a string array of Property names for which to fetch entities related to the object</param>
        /// <returns>RepositoryResultList of matching entities</returns>
        RepositoryResultList<T> Find(Expression<Func<T, bool>> filter, params string[] includes);
        /// <summary>
        /// Gets a set of entities based on the filter expression
        /// </summary>
        /// <param name="filter">The expression used to filter the entities</param>
        /// <param name="paging">The PagingCriteria object which defines the page of entities to return</param>
        /// <returns>RepositoryResultList of matching entities</returns>
        RepositoryResultList<T> Find(Expression<Func<T, bool>> filter, PagingCriteria paging);
        /// <summary>
        /// Gets a set of entities based on the filter expression
        /// </summary>
        /// <param name="filter">The expression used to filter the entities</param>
        /// <param name="paging">The PagingCriteria object which defines the page of entities to return</param>
        /// <param name="includes">a string array of Property names for which to fetch entities related to the object</param>
        /// <returns>RepositoryResultList of matching entities</returns>
        RepositoryResultList<T> Find(Expression<Func<T, bool>> filter, PagingCriteria paging, params string[] includes);
        /// <summary>
        /// Returns a single entity based on the expression
        /// </summary>
        /// <param name="filter">The Expression to use for finding a single entity</param>
        /// <returns>T</returns>
        RepositoryResultSingle<T> Single(Expression<Func<T, bool>> filter);

        RepositoryResultSingle<T> SingleAsNoTracking(Expression<Func<T, bool>> filter);
        /// <summary>
        /// Returns a single entity based on the expression
        /// </summary>
        /// <param name="filter">The Expression to use for finding a single entity</param>
        /// <param name="includes">a string array of Property names for which to fetch entities related to the object</param>
        /// <returns></returns>
        RepositoryResultSingle<T> Single(Expression<Func<T, bool>> filter, params string[] includes);

        /// <summary>
        /// Adds an entity to a datacontext
        /// </summary>
        /// <param name="entity">The entity to save</param>
        /// <returns>A RepositoryResult</returns>
        void Add(T entity);
        /// <summary>
        /// Modifies an entity in a datacontext
        /// </summary>
        /// <param name="entity">The entity to save</param>
        /// <returns>A RepositoryResult</returns>
        void Update(T entity);
        /// <summary>
        /// Deletes an entity from a datacontext
        /// </summary>
        /// <param name="entity">The entity to remove</param>
        /// <returns>A RepositoryResult</returns>
        void Delete(T entity);
        /// <summary>
        /// Deletes entityList from a datacontext
        /// </summary>
        /// <param name="entitys"></param>
        void DeleteAll(List<T> entitys);

        /// <summary>
        /// Gets all of an entity
        /// </summary>
        /// <returns>IEnumerable T"/></returns>
        IEnumerable<T> GetAllEntity();
        /// <summary>
        /// Finds a subset of an entity based on the supplied filter
        /// </summary>
        /// <param name="filter">The Expression to use to filter the entities</param>
        /// <returns>IEnumerable T</returns>
        IEnumerable<T> FindEntity(Expression<Func<T, bool>> filter);
        /// <summary>
        /// Finds a subset of an entity based on the supplied filter
        /// </summary>
        /// <param name="filter">The Expression to use to filter the entities</param>
        /// <param name="includes">a string array of Property names for which to fetch entities related to the object</param>
        /// <returns>IEnumerable T</returns>
        IEnumerable<T> FindEntity(Expression<Func<T, bool>> filter, params string[] includes);
        /// <summary>
        /// Finds a single entity based on the supplied filter
        /// </summary>
        /// <param name="filter">The Expression to use to filter to a single entity</param>
        /// <returns>T</returns>
        T SingleEntity(Expression<Func<T, bool>> filter);
        /// <summary>
        /// Finds a single entity based on the supplied filter
        /// </summary>
        /// <param name="filter">The Expression to use to filter to a single entity</param>
        /// <param name="includes">a string array of Property names for which to fetch entities related to the object</param>
        /// <returns>T</returns>
        T SingleEntity(Expression<Func<T, bool>> filter, params string[] includes);

        /// <summary>
        /// 获得存储过程结果集
        /// </summary>
        /// <param name="storedProc"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        //ResultsList CallStoredProc(StoredProc<T> storedProc, T parms);

        RepositoryResultList<T> QueryByESql(string esql);

        RepositoryResultList<T> QueryByESql(string esql, PagingCriteria paging);

        IEnumerable<T> QueryCustomerObjectByESql(string esql);

    }
}
